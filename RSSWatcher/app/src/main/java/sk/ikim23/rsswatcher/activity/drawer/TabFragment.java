package sk.ikim23.rsswatcher.activity.drawer;

import android.content.Context;
import android.content.Intent;
import android.os.Bundle;
import android.support.annotation.Nullable;
import android.support.v4.app.Fragment;
import android.support.v7.view.ActionMode;
import android.support.v7.widget.DividerItemDecoration;
import android.support.v7.widget.LinearLayoutManager;
import android.support.v7.widget.RecyclerView;
import android.support.v7.widget.SimpleItemAnimator;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;

import java.util.Date;

import sk.ikim23.rsswatcher.R;
import sk.ikim23.rsswatcher.U;
import sk.ikim23.rsswatcher.activity.feed.FeedActivity;
import sk.ikim23.rsswatcher.data.DbHelper;
import sk.ikim23.rsswatcher.data.FeedProvider;

public class TabFragment extends Fragment implements RecyclerViewAdapter.OnItemClickListener, CurrentChannelLoader.OnCurrentChannelLoadListener, RecyclerViewBulkAction.OnActionModeListener {

    private DrawerActivity activity;
    private RecyclerView recyclerView;
    private RecyclerViewAdapter recyclerViewAdapter;
    private FeedLoaderCallbacks feedLoader;
    private ActionMode actionMode;
    private boolean bulkActionEnabled = true;
    private String matchQuery;

    static TabFragment getInstance(int feedLoaderId, String feedQueryWhere) {
        Bundle bundle = new Bundle();
        bundle.putInt(U.EXTRA_FEED_LOADER_ID, feedLoaderId);
        bundle.putString(U.EXTRA_WHERE_CLAUSE, feedQueryWhere);
        TabFragment instance = new TabFragment();
        instance.setArguments(bundle);
        return instance;
    }

    @Override
    public void onAttach(Context context) {
        super.onAttach(context);
        this.activity = (DrawerActivity) context;
    }

    @Nullable
    @Override
    public View onCreateView(LayoutInflater inflater, @Nullable ViewGroup container, @Nullable Bundle savedInstanceState) {
        View rootView = inflater.inflate(R.layout.fragment_tab, container, false);
        recyclerView = (RecyclerView) rootView.findViewById(R.id.recycler_view);
        initRecyclerView();
        CurrentChannelLoader.getInstance().addListener(this);
        return rootView;
    }

    @Override
    public void onStart() {
        super.onStart();
        CurrentChannelLoader.loadCurrentChannel(activity);
    }

    @Override
    public void onDestroy() {
        super.onDestroy();
        CurrentChannelLoader.getInstance().removeListener(this);
    }

    // ****************************************
    // LIST BULK ACTION
    // ****************************************

    @Override
    public void onListItemClick(View view, int position, long id) {
        if (actionMode == null) {
            Intent intent = new Intent(activity, FeedActivity.class);
            String currentChannelName = CurrentChannelLoader.getInstance().getCurrentChannelName();
            intent.putExtra(U.EXTRA_CHANNEL_NAME, currentChannelName);
            intent.putExtra(U.EXTRA_POSITION, position);
            intent.putExtras(feedLoader.getBundle());
            startActivity(intent);
        } else {
            recyclerViewAdapter.setItemSelected(position, id, !recyclerViewAdapter.isItemSelected(id));
            actionMode.setTitle(recyclerViewAdapter.getSelectedItemCount() > 0 ? String.valueOf(recyclerViewAdapter.getSelectedItemCount()) : null);
        }
    }

    @Override
    public boolean onListItemLongClick(View view, int position, long id) {
        if (bulkActionEnabled) {
            if (actionMode == null) {
                activity.startSupportActionMode(new RecyclerViewBulkAction(activity, this, recyclerViewAdapter));
            }
            recyclerViewAdapter.setItemSelected(position, id, !recyclerViewAdapter.isItemSelected(id));
            actionMode.setTitle(recyclerViewAdapter.getSelectedItemCount() > 0 ? String.valueOf(recyclerViewAdapter.getSelectedItemCount()) : null);
            return true;
        }
        return false;
    }

    @Override
    public void setActionMode(ActionMode mode) {
        actionMode = mode;
        activity.setViewsEnabled(mode == null);
    }

    public void setBulkActionEnabled(boolean bulkActionEnabled) {
        this.bulkActionEnabled = bulkActionEnabled;
    }

    // ****************************************
    // DOWNLOAD DATA
    // ****************************************

    void setMatchQuery(String matchQuery) {
        this.matchQuery = matchQuery;
        restartFeedLoader();
    }

    @Override
    public void onNoChannelLoaded() {
        recyclerViewAdapter.swapCursor(null);
    }

    @Override
    public void onChannelLoaded(long channelId, String channelName, Date lastUpdate) {
        Log.d(getClass().getName(), "Channel loaded, where clause: '" + getArguments().getString(U.EXTRA_WHERE_CLAUSE) + "'");
        restartFeedLoader();
    }

    private void restartFeedLoader() {
        Bundle args = getArguments();
        Bundle loaderArgs = new Bundle();
        loaderArgs.putString(U.EXTRA_URI, "content://" + FeedProvider.AUTHORITY + "/" + (matchQuery == null ? FeedProvider.PATH_FEED : FeedProvider.PATH_MATCH));
        String where = String.format("%s = %d AND %s = 0 %s",
                DbHelper.FEED_CHANNEL_ID, CurrentChannelLoader.getInstance().getCurrentChannelId(),
                DbHelper.FEED_IS_DELETED,
                args.getString(U.EXTRA_WHERE_CLAUSE));
        if (matchQuery != null) {
            where += matchQuery;
        }
        loaderArgs.putString(U.EXTRA_WHERE_CLAUSE, where);
        int loaderId = args.getInt(U.EXTRA_FEED_LOADER_ID);
        activity.getSupportLoaderManager().restartLoader(loaderId, loaderArgs, feedLoader);
    }

    // ****************************************
    // SETUP METHODS
    // ****************************************

    private void initRecyclerView() {
        recyclerView.setLayoutManager(new LinearLayoutManager(activity));
        recyclerView.addItemDecoration(new DividerItemDecoration(activity, DividerItemDecoration.VERTICAL));
        // fix separator blinking
        RecyclerView.ItemAnimator animator = recyclerView.getItemAnimator();
        if (animator instanceof SimpleItemAnimator) {
            ((SimpleItemAnimator) animator).setSupportsChangeAnimations(false);
        }
        recyclerViewAdapter = new RecyclerViewAdapter(this, activity);
        recyclerView.setAdapter(recyclerViewAdapter);
        // initLoader will be called after setup of current channel
        feedLoader = new FeedLoaderCallbacks(activity, recyclerViewAdapter);
    }

}
