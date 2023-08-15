from math import sqrt
import subprocess
import cv2
import multiprocessing as mp
from ultralytics import YOLO
import numpy as np
CONFIDENCE_THRESHOLD = 0.5
GREEN = (0, 255, 0)
COLORS = np.random.uniform(0, 255, size=(100, 3))
class AIJob:
    
    def __init__(self,input_address , output_address) -> None:
        self.input_address = input_address
        self.output_address = output_address
        self.ffmpeg_process = None
        self.cc = -1
        self.model  = YOLO("yolov8n.pt")
        self.Done = "False"
        pass
    
    def _scale_frame(self,frame_orginal):
        if self.cc < 0 :
            w = self.video_capture.get(cv2.CAP_PROP_FRAME_WIDTH)
            h = self.video_capture.get(cv2.CAP_PROP_FRAME_HEIGHT)
            self.cc = sqrt((w*h) / 154600)
            self.W = int(w / self.cc)
            self.H = int(h / self.cc)
        return cv2.resize(frame_orginal, (self.W, self.H))

    def open_ffmpeg_stream_process(self,w,h):
        args = (
            "ffmpeg -re -stream_loop -1 -f rawvideo -pix_fmt "
            f"bgr24 -s {w}x{h} -i pipe:0 -vcodec h264 -preset ultrafast "
            "-f rtsp "+ self.output_address
        ).split()
        return subprocess.Popen(args, stdin=subprocess.PIPE)
    
    def process(self):
        self.video_capture = cv2.VideoCapture(self.input_address)
        self.video_capture.set(cv2.CAP_PROP_BUFFERSIZE, 2)
        self.ffmpeg_process = None
        i = 0
        try:
            detections = None
            while True:
                i +=1
                # Grab a single frame of video
                ret, frame_orginal = self.video_capture.read()
                if ret == True:
                    # frame = self._scale_frame(frame_orginal=frame_orginal)
                    frame = frame_orginal
                    if not self.ffmpeg_process:
                        fps = self.video_capture.get(cv2.CAP_PROP_FPS)
                        w = self.video_capture.get(cv2.CAP_PROP_FRAME_WIDTH)
                        h = self.video_capture.get(cv2.CAP_PROP_FRAME_HEIGHT)
                        self.ffmpeg_process = self.open_ffmpeg_stream_process(int(w),int(h))

                    if i % 3 ==0:
                        detections = self.model(frame)[0]
                    if detections is not None:
                        for data in detections.boxes.data.tolist():
                            # extract the confidence (i.e., probability) associated with the detection
                            confidence = data[4]

                            # filter out weak detections by ensuring the 
                            # confidence is greater than the minimum confidence
                            if float(confidence) < CONFIDENCE_THRESHOLD:
                                continue
                            # print(detections.names)
                            # if the confidence is greater than the minimum confidence,
                            # draw the bounding box on the frame
                            xmin, ymin, xmax, ymax = int(data[0]), int(data[1]), int(data[2]), int(data[3])
                            color = COLORS[int(data[5])]
                            cv2.rectangle(frame, (xmin, ymin) , (xmax, ymax), color, 2)
                            cv2.putText(frame, detections.names[data[5]], (xmin, ymin), cv2.FONT_HERSHEY_SIMPLEX, 1.5, color, 2)
                    

                    self.ffmpeg_process.stdin.write(frame.astype(np.uint8).tobytes())
                else:
                    break
        except  Exception as e:
            print(e)
        finally:
            # Release handle to the webcam
            self.video_capture.release()
            self.ffmpeg_process.stdin.close()
            self.ffmpeg_process.wait()
            print("Exiting Stream")


    def start(self):
        self.p = mp.Process(target = self.process)
        self.p.start()
    def wait(self):
        self.p.join()

    def kill(self):
        self.p.kill()
