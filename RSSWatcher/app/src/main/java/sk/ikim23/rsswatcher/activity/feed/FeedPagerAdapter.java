package sk.ikim23.rsswatcher.activity.feed;

import android.database.Cursor;
import android.net.Uri;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentManager;
import android.support.v4.app.FragmentStatePagerAdapter;
import android.support.v4.app.LoaderManager;
import android.support.v4.content.CursorLoader;
import android.support.v4.content.Loader;
import android.util.Log;
import android.view.ViewGroup;

import java.util.Date;

import sk.ikim23.rsswatcher.U;
import sk.ikim23.rsswatcher.data.DbHelper;
import sk.ikim23.rsswatcher.data.FeedProvider;

public class FeedPagerAdapter extends FragmentStatePagerAdapter {

    private Cursor cursor;
    private FeedFragment currentFragment;

    public FeedPagerAdapter(FragmentManager fragmentManager) {
        super(fragmentManager);
    }

    @Override
    public Fragment getItem(int position) {
        if (cursor != null && cursor.moveToPosition(position)) {
            Log.d("fragment loading", cursor.getExtras().toString());
            String date = U.MMM_d_HH_mm.format(new Date(cursor.getLong(cursor.getColumnIndex(DbHelper.FEED_PUB_DATE))));
            return FeedFragment.newInstance(
                    cursor.getLong(cursor.getColumnIndex(DbHelper.FEED_ID)),
                    cursor.getString(cursor.getColumnIndex(DbHelper.FEED_TITLE)),
                    date,
                    cursor.getString(cursor.getColumnIndex(DbHelper.FEED_DESCRIPTION)),
                    cursor.getString(cursor.getColumnIndex(DbHelper.FEED_LINK)));
        }
        return null;
    }

    @Override
    public int getCount() {
        return cursor != null ? cursor.getCount() : 0;
    }

    @Override
    public void setPrimaryItem(ViewGroup container, int position, Object object) {
        if (getCurrentFragment() != object) {
            currentFragment = ((FeedFragment) object);
        }
        super.setPrimaryItem(container, position, object);
    }

    public FeedFragment getCurrentFragment() {
        return currentFragment;
    }

    public void swapCursor(Cursor newCursor) {
        if (cursor != null) {
            cursor.close();
        }
        cursor = newCursor;
        notifyDataSetChanged();
    }

    public Cursor getCursor() {
        return cursor;
    }

    static class FeedPagerAdapterLoaderCallbacks implements LoaderManager.LoaderCallbacks<Cursor> {

        private final FeedActivity activity;
        private final FeedPagerAdapter adapter;
        private boolean isFirstLoad = true;

        FeedPagerAdapterLoaderCallbacks(FeedActivity activity, FeedPagerAdapter adapter) {
            this.activity = activity;
            this.adapter = adapter;
        }

        @Override
        public Loader<Cursor> onCreateLoader(int id, Bundle args) {
            Uri uri = Uri.parse(args.getString(U.EXTRA_URI));
            String path = uri.getPath().replaceAll("/", "");
            if (FeedProvider.PATH_FEED.equals(path) || FeedProvider.PATH_MATCH.equals(path)) {
                // to prevent change of feed list -> then must manually check IS_ARCHIVED field
                uri = Uri.parse("content://" + FeedProvider.AUTHORITY + "/" + FeedProvider.PATH_FEED_NO_UPDATE);
            }
            String where = args.getString(U.EXTRA_WHERE_CLAUSE);
            String sortOrder = DbHelper.FEED_PUB_DATE + " DESC";
            return new CursorLoader(activity, uri, new String[]{DbHelper.FEED_ID, DbHelper.FEED_TITLE, DbHelper.FEED_PUB_DATE, DbHelper.FEED_DESCRIPTION, DbHelper.FEED_LINK, DbHelper.FEED_IS_ARCHIVED, DbHelper.FEED_IS_READ}, where, null, sortOrder);
        }

        @Override
        public void onLoadFinished(Loader<Cursor> loader, Cursor data) {
            adapter.swapCursor(data);
            if (isFirstLoad && data != null) {
                activity.setCurrentItem(data);
                isFirstLoad = false;
            }
        }

        @Override
        public void onLoaderReset(Loader<Cursor> loader) {
        }
    }

}
