# OoyalaDownloader
A C# console application designed to download all videos from your Ooyala Backlot account. Accesses Ooyala's Backlot REST /v2/asset API to pull down a list of all your videos, loops though each video to get the original upload source media file, and downloads it. Will also create a CSV file to keep track of the orginal asset ID, File Name, Ooyala Name, and Description.

### Prerequisites
* Requires [Ooyala C# library](https://github.com/ooyala/csharp-v2-sdk)
* Requires Newtonsoft.Json

### Instructions
Once you import the Ooyala C# Library, and the Newtonsoft.Json Library; the only thing you need to configure is the OOyala API key, and Secret key on line 15. You can find both of those in your Ooyala Backlot account
```
private const string apiKey = "<Your Ooyala API Key>";
private const string secretKey = "<Your Ooyala Secret Key>";
```

### Options
You can add in filtering parameters for the backlot REST API start on line 21, currently the video paging size is set to 100 videos, and it will only return video assets. You could add filtering options to only download videos with a certain tag, or videos older then a certain date. For more information on [Ooyala's backlot Rest API](http://help.ooyala.com/video-platform/api/assets.html).
```
parameters.Add("limit", "100");
parameters.Add("where", "asset_type = 'video'");
```

### Bugs & Possible Errors
* Request limit exceeded - Ooyala restricts how many API calls any one user can make in a given time period. To deal with this the app will wait 60 seconds after a failed connection to try again. If it fails again the app will crash. If this happens, simply restart the app. It will check for any video files already downloaded and skip them.
* Request Expired - Ooyala Stores API connections for an hour, after that expires, you will get an error. Simply restart the app. It will check for any video files already downloaded and skip them.
* File Download Remotely terminated - Not sure why this one happens, but after the app finishes, restart it to download any missing videos.

Support online content. [Buy a Coffee for Alvcard with Ko-fi.com](https://ko-fi.com/G2G65JKY)
