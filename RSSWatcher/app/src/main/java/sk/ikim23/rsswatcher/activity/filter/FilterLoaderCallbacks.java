package sk.ikim23.rsswatcher.activity.filter;

import android.content.Context;
import android.database.Cursor;
import android.net.Uri;
import android.os.Bundle;
import android.support.v4.app.LoaderManager;
import android.support.v4.content.CursorLoader;
import android.support.v4.content.Loader;
import android.util.Log;

import sk.ikim23.rsswatcher.activity.drawer.CurrentChannelLoader;
import sk.ikim23.rsswatcher.activity.drawer.SwapableAdapter;
import sk.ikim23.rsswatcher.data.DbHelper;
import sk.ikim23.rsswatcher.data.FeedProvider;

public class FilterLoaderCallbacks implements LoaderManager.LoaderCallbacks<Cursor> {

    private final Context context;
    private final SwapableAdapter adapter;

    public FilterLoaderCallbacks(Context context, SwapableAdapter adapter) {
        this.context = context;
        this.adapter = adapter;
    }

    @Override
    public Loader<Cursor> onCreateLoader(int id, Bundle args) {
        Uri uri = Uri.parse("content://" + FeedProvider.AUTHORITY + "/" + FeedProvider.PATH_FILTER);
        long channelId = CurrentChannelLoader.getInstance().getCurrentChannelId();
        String where = DbHelper.FILTER_CHANNEL_ID + "=" + channelId;
        return new CursorLoader(context, uri, new String[]{DbHelper.FILTER_ID, DbHelper.FILTER_MATCH_QUERY}, where, null, null);
    }

    @Override
    public void onLoadFinished(Loader<Cursor> loader, Cursor data) {
        Log.d(getClass().getName(), "load finished: " + data.getCount());
        adapter.swapCursor(data);
    }

    @Override
    public void onLoaderReset(Loader<Cursor> loader) {
    }
}
