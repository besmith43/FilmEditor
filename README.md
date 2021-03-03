#FilmEditor

This tool was made for the purpose of automating some of the processes required for maniplating video files for my home media server.

##Requirements

This application requires ffmpeg to be in your system's path.  For Scaling only, it requires [video2x](https://github.com/k4yt3x/video2x) and is only tested to work with the Windows binaries.

##Commands

There are several commands that this application performs

* new
* edit
* split
* rename
* scale

####New Command

Usage:  FilmEditor new [OPTION]

    -e | --edit <mp4 file>           Create csv for edit command
    -s | --split <mp4 file>          Create csv for split command
    --season <int>                   (Optional) Pass in the season number

####Edit Command

Usage:  FilmEditor edit [OPTION]

    -c | --csv <csv file>            Pass in csv containing edit information
	-o | --output <destionation folder>  (Optional) Set Destionation Folder

####Split Command

Usage:  FilmEditor new [OPTION]

    -c | --csv <csv file>        Pass in csv containing split information

####Rename Command

Usage:  FilmEditor new [OPTION]

    -s | --source <source folder>               Pass in the folder with the original files that need to be renamed

####Scale Command

Usage:  FilmEditor scale [OPTION]

    -e | --exe <video2x executable file>            Pass in the path to the video2x executable file
	    -m | --movie <single video file>            Pass in the file to be scaled up and down accordingly
    -t | -tv <folder path>            Path in the folder to be scaled up and down accordingly
    -o | --output <destionation folder>  (Optional) Set Destionation Folder
    -n | --new                        Switch statement to make a new folder per video converted
    --4k                              Switch to select 4K scale option
	--2k                              Switch to select 2K scale option
    --1080p                           Switch to select 1080p scale option
    --720p                            Switch to select 720p scale option
    --480p                            Switch to select 480p scale option
	-o | --output <destination folder>  (Optional) Specify the destination folder for the renamed files
    --showname <TV show name>                   Pass in the TV Show episode files being renamed
	-d | --delete                                               Delete the original copy of the files




