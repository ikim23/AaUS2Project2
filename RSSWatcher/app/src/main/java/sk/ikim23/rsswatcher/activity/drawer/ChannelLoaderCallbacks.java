package sk.ikim23.rsswatcher.activity.drawer;

import android.app.Activity;
import android.database.Cursor;
import android.graphics.drawable.Drawable;
import android.net.Uri;
import android.os.Bundle;
import android.support.design.widget.NavigationView;
import android.support.v4.app.LoaderManager;
import android.support.v4.content.ContextCompat;
import android.support.v4.content.CursorLoader;
import android.support.v4.content.Loader;
import android.util.Log;
import android.util.SparseArray;
import android.view.Menu;
import android.view.MenuItem;
import android.widget.TextView;

import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;

import sk.ikim23.rsswatcher.R;
import sk.ikim23.rsswatcher.U;
import sk.ikim23.rsswatcher.data.DbHelper;
import sk.ikim23.rsswatcher.data.FeedProvider;
import sk.ikim23.rsswatcher.service.FaviconDownloadTask;

public class ChannelLoaderCallbacks implements LoaderManager.LoaderCallbacks<Cursor> {

    private final Activity activity;
    private final NavigationView navigationView;
    private final SparseArray<Long> resIdToDatabaseId = new SparseArray<>();

    public ChannelLoaderCallbacks(Activity activity, NavigationView navigationView) {
        this.activity = activity;
        this.navigationView = navigationView;
        navigationView.setItemIconTintList(null);
//        navigationView.setItemBackgroundResource(R.drawable.nav_item_background);
    }

    @Override
    public Loader<Cursor> onCreateLoader(int id, Bundle args) {
        Log.d(U.LOG_TAG, "Channel Loader: onCreateLoader");
        return new CursorLoader(activity, FeedProvider.URI_CHANNEL_JOIN_FEED,
                new String[]{DbHelper.CHANNEL_ID, DbHelper.CHANNEL_NAME, DbHelper.CHANNEL_URL, DbHelper.CHANNEL_ICON_PATH}, null, null, DbHelper.CHANNEL_NAME);
    }

    @Override
    public void onLoadFinished(Loader<Cursor> loader, Cursor cursor) {
        Log.d(U.LOG_TAG, "Channel Loader: onLoadFinished");
        if (cursor == null) {
            Log.d(U.LOG_TAG, "CURSOR NULL");
            return;
        }
        Menu menu = navigationView.getMenu();
        menu.setGroupCheckable(R.id.nav_group_channels, true, false);
        // clear previous menu items, to avoid duplicity
        int resId;
        for (int i = 0; i < resIdToDatabaseId.size(); i++) {
            resId = resIdToDatabaseId.keyAt(i);
            menu.removeItem(resId);
        }
        resIdToDatabaseId.clear();

        Log.d(U.LOG_TAG, "Cursor size: " + cursor.getCount());
        resId = 1;
        cursor.moveToFirst();
        while (!cursor.isAfterLast()) {
            long rowId = cursor.getLong(cursor.getColumnIndex(DbHelper.CHANNEL_ID));
            Log.d(U.LOG_TAG, rowId + " " + cursor.getString(cursor.getColumnIndex(DbHelper.CHANNEL_NAME)) + " " + cursor.getString(cursor.getColumnIndex(DbHelper.CHANNEL_ICON_PATH)));

            MenuItem item = menu.add(R.id.nav_group_channels, resId, Menu.NONE, cursor.getString(cursor.getColumnIndex(DbHelper.CHANNEL_NAME)));

            // set feed icon
            String iconPath = cursor.getString(cursor.getColumnIndex(DbHelper.CHANNEL_ICON_PATH));
            if (iconPath != null) {
                try {
                    Drawable iconDrawable = Drawable.createFromStream(new FileInputStream(new File(activity.getFilesDir().getPath() + cursor.getString(3))), null);
                    item.setIcon(iconDrawable);
                } catch (IOException e) {
                    item.setIcon(ContextCompat.getDrawable(activity, R.drawable.ic_rss_feed_gray_24dp));
                    e.printStackTrace();
                }
            } else {
                item.setIcon(ContextCompat.getDrawable(activity, R.drawable.ic_rss_feed_gray_24dp));
                new FaviconDownloadTask(activity, rowId, cursor.getString(cursor.getColumnIndex(DbHelper.CHANNEL_URL))).execute();
            }
            // set count of unread feeds
            int unread = cursor.getInt(4);
            if (unread > 0) {
                item.setActionView(R.layout.drawer_counter);
                TextView newsCounter = (TextView) item.getActionView();
                newsCounter.setText(String.valueOf(unread));
            }

            resIdToDatabaseId.put(resId, rowId);
            cursor.moveToNext();
            resId++;
        }
        // do not close cursor, it would stop calling callbacks
    }

    @Override
    public void onLoaderReset(Loader<Cursor> loader) {
        Log.d(U.LOG_TAG, "Channel Loader: onLoaderReset");
    }

    public long getChannelId(int resId) {
        return resIdToDatabaseId.get(resId);
    }

    public MenuItem getMenuItem(Long channelId) {
        Menu menu = navigationView.getMenu();
        int idx = resIdToDatabaseId.indexOfValue(channelId);
        return menu.findItem(resIdToDatabaseId.keyAt(idx));
    }
}
