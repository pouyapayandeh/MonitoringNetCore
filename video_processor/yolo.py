from math import sqrt
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
        self.video_capture = None
        self.fourcc  = cv2.VideoWriter_fourcc('H', '2', '6', '4')
        self.cc = -1
        self.model  = YOLO("yolov8n.pt")
        self.status = mp.Array('c', b"waiting")
        pass
    
    def _scale_frame(self,frame_orginal):
        if self.cc < 0 :
            w = self.video_capture.get(cv2.CAP_PROP_FRAME_WIDTH)
            h = self.video_capture.get(cv2.CAP_PROP_FRAME_HEIGHT)
            self.cc = sqrt((w*h) / 154600)
            self.W = int(w / self.cc)
            self.H = int(h / self.cc)
        return cv2.resize(frame_orginal, (self.W, self.H))

    def process(self):
        self.video_capture = cv2.VideoCapture(self.input_address)
        self.writer = None
        self.status.value =  b"running"
        print("Starting")
        try:
            while True:
                print("Grab Frame")
                # Grab a single frame of video
                ret, frame_orginal = self.video_capture.read()
                if ret == True:
                    frame = frame_orginal
                    if not self.writer:
                        fps = self.video_capture.get(cv2.CAP_PROP_FPS)
                        w = self.video_capture.get(cv2.CAP_PROP_FRAME_WIDTH)
                        h = self.video_capture.get(cv2.CAP_PROP_FRAME_HEIGHT)
                        self.writer = cv2.VideoWriter(self.output_address, self.fourcc,fps, (int(w), int(h)))
                        
                    print("Run Model")
                    detections = self.model(frame)[0]
                    for data in detections.boxes.data.tolist():
                        confidence = data[4]
                        if float(confidence) < CONFIDENCE_THRESHOLD:
                            continue
                        xmin, ymin, xmax, ymax = int(data[0]), int(data[1]), int(data[2]), int(data[3])
                        color = COLORS[int(data[5])]
                        cv2.rectangle(frame, (xmin, ymin) , (xmax, ymax), color, 2)
                        cv2.putText(frame, detections.names[data[5]], (xmin, ymin), cv2.FONT_HERSHEY_SIMPLEX, 1.5, color,2)
                

                    self.writer.write(frame)
                else:
                    break
        except:
            self.status.value =  b"error"
        finally:
            self.video_capture.release()
            self.writer.release() 

        if self.status != "error":
            self.status.value =  b"done"

    def start(self):
        self.p = mp.Process(target = self.process)
        self.p.start()
    def wait(self):
        self.p.join()
