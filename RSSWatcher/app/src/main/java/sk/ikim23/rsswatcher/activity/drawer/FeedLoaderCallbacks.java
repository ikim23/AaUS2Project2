package sk.ikim23.rsswatcher.activity.drawer;

import android.content.Context;
import android.database.Cursor;
import android.net.Uri;
import android.os.Bundle;
import android.support.v4.app.LoaderManager;
import android.support.v4.content.CursorLoader;
import android.support.v4.content.Loader;
import android.util.Log;

import sk.ikim23.rsswatcher.U;
import sk.ikim23.rsswatcher.data.DbHelper;

public class FeedLoaderCallbacks implements LoaderManager.LoaderCallbacks<Cursor> {

    private final Context context;
    private final SwapableAdapter adapter;
    private Bundle bundle;

    public FeedLoaderCallbacks(Context context, SwapableAdapter adapter) {
        this.context = context;
        this.adapter = adapter;
    }

    @Override
    public Loader<Cursor> onCreateLoader(int id, Bundle args) {
        bundle = args;
        Uri uri = Uri.parse(args.getString(U.EXTRA_URI));
        String where = args.getString(U.EXTRA_WHERE_CLAUSE);
        String sortOrder = DbHelper.FEED_PUB_DATE + " DESC";
        Log.d(getClass().getName(), "Feed Loader: onCreateLoader, uri: " + uri + ", where: " + where);
        return new CursorLoader(context, uri, new String[]{DbHelper.FEED_ID, DbHelper.FEED_TITLE, DbHelper.FEED_DESCRIPTION_NO_HTML, DbHelper.FEED_PUB_DATE, DbHelper.FEED_IS_READ, DbHelper.FEED_IS_ARCHIVED}, where, null, sortOrder);
    }

    @Override
    public void onLoadFinished(Loader<Cursor> loader, Cursor data) {
        Log.d(U.LOG_TAG, "Feed Loader: onLoadFinished");
        adapter.swapCursor(data);
    }

    @Override
    public void onLoaderReset(Loader<Cursor> loader) {
        Log.d(U.LOG_TAG, "Feed Loader: onLoaderReset");
    }

    public Bundle getBundle() {
        return bundle;
    }
}
