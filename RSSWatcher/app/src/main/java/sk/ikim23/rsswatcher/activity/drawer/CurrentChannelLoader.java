package sk.ikim23.rsswatcher.activity.drawer;

import android.content.Context;
import android.database.Cursor;
import android.net.Uri;
import android.os.AsyncTask;
import android.util.Log;

import java.util.Comparator;
import java.util.Date;
import java.util.Iterator;
import java.util.TreeSet;

import sk.ikim23.rsswatcher.U;
import sk.ikim23.rsswatcher.data.DbHelper;
import sk.ikim23.rsswatcher.data.FeedProvider;

public class CurrentChannelLoader {

    public interface OnCurrentChannelLoadListener {
        void onChannelLoaded(long channelId, String channelName, Date lastUpdate);

        void onNoChannelLoaded();
    }

    private static CurrentChannelLoader instance;

    public static CurrentChannelLoader getInstance() {
        if (instance == null) {
            instance = new CurrentChannelLoader();
        }
        return instance;
    }

    public static void loadCurrentChannel(final Context context) {
        if (instance != null && instance.listeners.size() > 0) {
            new ChannelLoader(context).execute();
        }
    }

    private final TreeSet<OnCurrentChannelLoadListener> listeners = new TreeSet<>(new Comparator<OnCurrentChannelLoadListener>() {
        @Override
        public int compare(OnCurrentChannelLoadListener l1, OnCurrentChannelLoadListener l2) {
            return Integer.valueOf(l1.hashCode()).compareTo(l2.hashCode());
        }
    });
    private long currentChannelId = U.CHANNEL_ID_UNDEFINED;
    private String currentChannelName;
    private Date currentChannelLastUpdate;

    public void addListener(OnCurrentChannelLoadListener listener) {
        if (listener != null) {
            listeners.add(listener);
        }
    }

    public void removeListener(OnCurrentChannelLoadListener listener) {
        if (listener != null) {
            listeners.remove(listener);
        }
    }

    public long getCurrentChannelId() {
        return currentChannelId;
    }

    public void setCurrentChannelId(long channelId) {
        currentChannelId = channelId;
    }

    public String getCurrentChannelName() {
        return currentChannelName;
    }

    public Date getCurrentChannelLastUpdate() {
        return currentChannelLastUpdate;
    }

    private static class ChannelLoader extends AsyncTask<Void, Void, Void> {

        private final Context context;

        ChannelLoader(Context context) {
            this.context = context;
        }

        @Override
        protected Void doInBackground(Void... v) {
            if (instance.currentChannelId == U.CHANNEL_ID_UNDEFINED) {
                instance.currentChannelId = U.getPrefLastChannelId(context);
            }
            // load last opened channel data
            Uri uri = Uri.parse("content://" + FeedProvider.AUTHORITY + "/" + DbHelper.TABLE_CHANNEL + "/" + instance.currentChannelId);
            setChannelData(uri);
            // if channel does not exists, choose first available
            if (instance.currentChannelId == U.CHANNEL_ID_UNDEFINED) {
                uri = Uri.parse("content://" + FeedProvider.AUTHORITY + "/" + DbHelper.TABLE_CHANNEL);
                setChannelData(uri);
            }
            // save data
            U.setPrefLastChannelId(context, instance.currentChannelId);
            return null;
        }

        private void setChannelData(Uri uri) {
            Cursor cursor = context.getContentResolver().query(uri, new String[]{DbHelper.CHANNEL_ID, DbHelper.CHANNEL_NAME, DbHelper.CHANNEL_LAST_UPDATE}, null, null, DbHelper.CHANNEL_NAME);
            if (cursor != null) {
                if (cursor.getCount() > 0) {
                    cursor.moveToFirst();
                    instance.currentChannelId = cursor.getLong(cursor.getColumnIndex(DbHelper.CHANNEL_ID));
                    instance.currentChannelName = cursor.getString(cursor.getColumnIndex(DbHelper.CHANNEL_NAME));
                    long lastUpdate = cursor.getLong(cursor.getColumnIndex(DbHelper.CHANNEL_LAST_UPDATE));
                    if (lastUpdate != 0) {
                        instance.currentChannelLastUpdate = new Date(lastUpdate);
                    }
                } else {
                    instance.currentChannelId = U.CHANNEL_ID_UNDEFINED;
                    instance.currentChannelName = null;
                    instance.currentChannelLastUpdate = null;
                }
                cursor.close();
            }
        }

        @Override
        protected void onPostExecute(Void v) {
            Log.d(getClass().getName(),
                    "Current channel: id: " + instance.currentChannelId +
                            ", name: " + instance.currentChannelName +
                            ", lastUpdate: " + instance.currentChannelLastUpdate);
            Iterator<OnCurrentChannelLoadListener> iterator = instance.listeners.iterator();
            OnCurrentChannelLoadListener listener;
            while (iterator.hasNext()) {
                listener = iterator.next();
                if (listener != null) {
                    if (instance.currentChannelId != U.CHANNEL_ID_UNDEFINED) {
                        listener.onChannelLoaded(instance.currentChannelId, instance.currentChannelName, instance.currentChannelLastUpdate);
                    } else {
                        listener.onNoChannelLoaded();
                    }
                }
            }
        }
    }

}
