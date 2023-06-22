from flask import Flask, request, send_file, jsonify
from flask_restful import Resource, Api
import os

app = Flask(__name__)
api = Api(app)


class VideoToVideoAPI(Resource):
    def post(self):
        print(request.files)
        file = request.files['video']
        file.save(file.filename)
        return send_file(file.filename, mimetype='video/mp4')


class VideoToTextAPI(Resource):
    def post(self):
        print(request.files)
        file = request.files['video']
        file.save(file.filename)
        return jsonify({'message': file.filename})


api.add_resource(VideoToVideoAPI, '/video')
api.add_resource(VideoToTextAPI, '/text')

if __name__ == '__main__':
    app.run(debug=True, host='0.0.0.0', port=8000)
