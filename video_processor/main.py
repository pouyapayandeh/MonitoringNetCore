from flask import Flask, Response, make_response, request, send_file, jsonify
from flask_restful import Resource, Api
import requests
from yolo import AIJob
from rtsp import AIJob as RtpsJob
import os
import multiprocessing as mp
app = Flask(__name__)
api = Api(app)

rtps = None
jobs = dict()
files = dict()


def download_file(url):
    local_filename = url.split('/')[-1]
    # NOTE the stream=True parameter below
    with requests.get(url, stream=True) as r:
        r.raise_for_status()
        with open(local_filename, 'wb') as f:
            for chunk in r.iter_content(chunk_size=8192): 
                # If you have chunk encoded response uncomment if
                # and set chunk_size parameter to None.
                #if chunk: 
                f.write(chunk)
    return local_filename

class ProcessVideoBlockingNonBlocking(Resource):
    def post(self):
        file_url = request.form['url']
        print(file_url)
        file_name = download_file(file_url)
        print(file_name)

        output_file = "p_"+file_name+".mp4"

        job = AIJob(file_name,output_file)
        jobs[str(hash(job))] = job.status
        files[str(hash(job))] =output_file
        job.start()
        return hash(job)

class Status(Resource):
    def get(self):
        print(request.args)
        print(jobs.keys())
        job_id = request.args["id"]

        if job_id in jobs:
            response = make_response(jobs[job_id].value.decode("utf-8"), 200)
            response.mimetype = "text/plain"
            return response
        return "key not found",404

class File(Resource):
    def get(self):
        print(request.args)
        print(jobs.keys())
        job_id = request.args["id"]
        if job_id in files:
            return send_file(files[job_id], mimetype='video/mp4')
        return "key not found",404
    
class RTPS(Resource):
    def post(self):
        global rtps


        print(request.form)
        read_url = request.form.get('read_url')
        print(read_url)
        write_url = request.form.get('write_url')
        print(write_url)

        
        if rtps is not None:
            print("Kill job")
            rtps.kill()
            
        if "stop" in request.form :
            rtps = None
        else:
            job = RtpsJob(read_url,write_url)
            rtps = job
            job.start()

        return "ok"

api.add_resource(ProcessVideoBlockingNonBlocking, '/process')
api.add_resource(Status, '/status')
api.add_resource(File, '/file')
api.add_resource(RTPS, '/rtps')


if __name__ == '__main__':
    mp.set_start_method('spawn')
    app.run(debug=True, host='0.0.0.0', port=8000)
