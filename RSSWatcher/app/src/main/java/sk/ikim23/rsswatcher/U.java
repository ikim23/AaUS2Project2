package sk.ikim23.rsswatcher;

import android.content.Context;
import android.content.SharedPreferences;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;
import android.preference.PreferenceManager;
import android.text.TextUtils;
import android.util.Log;

import org.jsoup.Jsoup;

import java.net.MalformedURLException;
import java.net.URL;
import java.text.Normalizer;
import java.text.SimpleDateFormat;

public class U {

    public static final long FILTER_ID_DEFAULT = -1L;
    public static final String EXTRA_FILTER_ID = "filter_id";
    public static final String EXTRA_SERVICE_EXECUTED = "service_executed";
    public static final int DELETE_AFTER_DAYS = 14;
    public static final SimpleDateFormat MMM_d_HH_mm = new SimpleDateFormat("MMM d, HH:mm");
    public static final SimpleDateFormat MMM_d = new SimpleDateFormat("MMM d");
    public static final SimpleDateFormat HH_mm = new SimpleDateFormat("HH:mm");
    public static final String LOG_TAG = "ikim23";
    public static final String ASSETS_BASE_URL = "file:///android_asset/";
    public static final String ICONS_DIR = "/icons";
    // broadcasts
    public static final String DOWNLOAD_FINISHED = "download finished";

    // shared preferences
    public static final long CHANNEL_ID_UNDEFINED = -1L;
    public static final String PREFS_LAST_CHANNEL_ID = "last_channel_id";
    public static final String PREFS_SYNC_FREQUENCY = "sync_frequency";
    public static final String PREFS_DELETE_FREQUENCY = "delete_frequency";
    public static final String PREFS_ONLY_WIFI = "only_wifi";
    public static final String EXTRA_URI = "uri";
    public static final String EXTRA_WHERE_CLAUSE = "where_clause";
    public static final String EXTRA_FEED_LOADER_ID = "feed_loader_id";
    public static final String EXTRA_DATE = "date";
    public static final String EXTRA_DESCRIPTION = "description";
    public static final String EXTRA_LINK = "link";
    public static final String EXTRA_ID = "id";
    public static final String EXTRA_UPDATE = "update";
    private static boolean serviceScheduledBefore = false;

    // feed activity extras
    public static final String EXTRA_CHANNEL_NAME = "channel_name";
    // download service extras
    public static final String EXTRA_CHANNEL_ID = "channelId";
    public static final String EXTRA_SCHEDULE_NEXT_SERVICE = "scheduleNextService";
    public static final String EXTRA_ERROR = "errors";
    public static final String EXTRA_POSITION = "position";
    public static final String EXTRA_SEARCH_QUERY = "search";
    public static final String EXTRA_CALLER = "caller";
    public static final String EXTRA_TITLE = "title";

    public static boolean isValidURL(String link) {
        try {
            new URL(link);
        } catch (MalformedURLException e) {
            return false;
        }
        return true;
    }

    public static boolean isConnected(Context context) {
        ConnectivityManager cm = (ConnectivityManager) context.getSystemService(Context.CONNECTIVITY_SERVICE);
        NetworkInfo netInfo = cm.getActiveNetworkInfo();
        if (netInfo != null) {
            Log.d("Utils", "only wifi: " + getPrefOnlyWifi(context));
            Log.d("Utils", "net info: " + netInfo);
            if (netInfo.isConnectedOrConnecting()) {
                return !getPrefOnlyWifi(context) || netInfo.getType() == ConnectivityManager.TYPE_WIFI;
            }
        } else {
            Log.d("Utils", "ConnectivityManager.getActiveNetworkInfo is null");
        }
        return false;
    }

    public static long minToMillis(int min) {
        return 1000 * 60 * min;
    }

    public static String normalize(String s) {
        s = Jsoup.parse(s).text();
        s = Normalizer.normalize(s, Normalizer.Form.NFD);
        s = s.replaceAll("\\p{M}", "");
        return s.toLowerCase();
    }

    public static String toMatchQuery(String s) {
        return TextUtils.join(" OR ", U.normalize(s).split(" "));
    }

    public static long getPrefLastChannelId(Context context) {
        SharedPreferences prefs = PreferenceManager.getDefaultSharedPreferences(context);
        return prefs.getLong(PREFS_LAST_CHANNEL_ID, CHANNEL_ID_UNDEFINED);
    }

    public static void setPrefLastChannelId(Context context, long channelId) {
        SharedPreferences prefs = PreferenceManager.getDefaultSharedPreferences(context);
        SharedPreferences.Editor editor = prefs.edit();
        editor.putLong(PREFS_LAST_CHANNEL_ID, channelId);
        editor.commit();
    }

    public static int getPrefSyncFrequency(Context context) {
        SharedPreferences prefs = PreferenceManager.getDefaultSharedPreferences(context);
        return Integer.parseInt(prefs.getString(PREFS_SYNC_FREQUENCY, context.getString(R.string.pref_default_value_sync_frequency)));
    }

    public static int getPrefSyncFrequencyNever(Context context) {
        return Integer.parseInt(context.getString(R.string.pref_sync_frequency_never));
    }

    public static int getPrefDeleteFrequency(Context context) {
        SharedPreferences prefs = PreferenceManager.getDefaultSharedPreferences(context);
        return Integer.parseInt(prefs.getString(PREFS_DELETE_FREQUENCY, context.getString(R.string.pref_default_value_delete_frequency)));
    }

    public static boolean getPrefOnlyWifi(Context context) {
        SharedPreferences prefs = PreferenceManager.getDefaultSharedPreferences(context);
        return prefs.getBoolean(PREFS_ONLY_WIFI, true);
    }

    public static synchronized boolean scheduleServiceIfNotBefore() {
        if (!serviceScheduledBefore) {
            serviceScheduledBefore = true;
            return true;
        }
        return false;
    }

    public static int longToInt(long l) {
        if (l > Integer.MAX_VALUE)
            return longToInt(l - Integer.MAX_VALUE);
        if (l < Integer.MIN_VALUE)
            return longToInt(l + Integer.MAX_VALUE);
        return (int) l;
    }

}
