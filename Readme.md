# FFMPEGScreenRecorder
FFMPEGScreenRecorder is a Desktop Screen Recorder Windows application based on [FFmpeg](https://www.ffmpeg.org/) multimedia framework.

### Pre-requisites
* Need to install FFmpeg library.

### Steps to set-up FFmpeg on System
* Go to [https://ffmpeg.org/download.html](https://ffmpeg.org/download.html)
* Click the <b>"Windows logo"</b>
* Click on <b>"Windows builds from gyan.dev"</b>
* Scroll down to the <b>"git"</b> section.
* Click the link to download <b>"ffmpeg-git-full.7z"</b>
* Extract the downloaded file into <b>"C:\" Drive</b>
* Rename the extracted folder to <b>"FFmpeg"</b>
* Right-click the Windows/Start menu and select <b>"Command prompt (Admin)"</b>
* Type the below command in CMD to add FFmpeg to the path.
```
setx /m PATH "C:\FFmpeg\bin;%PATH%"
```
* To check FFmpeg is properly installed type below command in CMD
```
ffmpeg -version
```
For Detail installation visit this:- [(How to Install FFmpeg on Windows)](https://www.wikihow.com/Install-FFmpeg-on-Windows)  

### Latest Release
[FFMPEGScreenRecorder(Beta)](https://github.com/Hardik0707/FFMPEGScreenRecorder/releases/tag/v0.1)

### License
This project is licensed under the MIT - see the [LICENSE](./LICENSE) file for details