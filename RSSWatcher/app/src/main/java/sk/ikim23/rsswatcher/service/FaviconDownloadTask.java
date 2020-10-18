package sk.ikim23.rsswatcher.service;

import android.app.Activity;
import android.content.ContentValues;
import android.net.Uri;
import android.os.AsyncTask;

import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.net.HttpURLConnection;
import java.net.URL;

import sk.ikim23.rsswatcher.U;
import sk.ikim23.rsswatcher.data.DbHelper;
import sk.ikim23.rsswatcher.data.FeedProvider;

public class FaviconDownloadTask extends AsyncTask<Void, Void, String> {

    private final Activity activity;
    private final long channelId;
    private final String channelUrl;

    public FaviconDownloadTask(Activity activity, long channelId, String channelUrl) {
        this.activity = activity;
        this.channelId = channelId;
        this.channelUrl = channelUrl;
    }

    @Override
    protected String doInBackground(Void... params) {
        String iconPath = null;
        HttpURLConnection conn = null;
        InputStream is = null;
        FileOutputStream fos = null;
        try {
            URL faviconUrl = new URL(channelUrl);
            faviconUrl = new URL(faviconUrl.getProtocol() + "://" + faviconUrl.getHost() + "/favicon.ico");
            // create icons directory
            File iconsDir = new File(activity.getFilesDir().getPath() + U.ICONS_DIR);
            if (!iconsDir.exists()) {
                iconsDir.mkdir();
            }
            iconPath = U.ICONS_DIR + "/" + channelId + ".ico";

            conn = (HttpURLConnection) faviconUrl.openConnection();
            is = conn.getInputStream();
            fos = new FileOutputStream(new File(activity.getFilesDir().getPath() + iconPath));
            byte[] buff = new byte[1024];
            int read;
            while ((read = is.read(buff)) != -1) {
                fos.write(buff, 0, read);
            }
        } catch (IOException e) {
            e.printStackTrace();
            iconPath = null;
        } finally {
            if (fos != null) {
                try {
                    fos.close();
                } catch (IOException e) {
                    e.printStackTrace();
                    iconPath = null;
                }
            }
            if (is != null) {
                try {
                    is.close();
                } catch (IOException e) {
                    e.printStackTrace();
                    iconPath = null;
                }
            }
            if (conn != null) {
                conn.disconnect();
            }
        }
        return iconPath;
    }

    @Override
    protected void onPostExecute(String iconPath) {
        if (iconPath != null) {
            ContentValues values = new ContentValues();
            values.put(DbHelper.CHANNEL_ICON_PATH, iconPath);
            Uri uri = Uri.parse("content://" + FeedProvider.AUTHORITY + "/" + FeedProvider.PATH_CHANNEL + "/" + channelId);
            activity.getContentResolver().update(uri, values, null, null);
        }
    }
}
