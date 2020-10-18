package sk.ikim23.rsswatcher.activity.notification;

import android.content.Intent;
import android.os.Bundle;
import android.support.v7.app.ActionBar;
import android.support.v7.app.AppCompatActivity;
import android.support.v7.widget.DividerItemDecoration;
import android.support.v7.widget.LinearLayoutManager;
import android.support.v7.widget.RecyclerView;
import android.support.v7.widget.SimpleItemAnimator;
import android.support.v7.widget.Toolbar;
import android.view.View;

import sk.ikim23.rsswatcher.R;
import sk.ikim23.rsswatcher.U;
import sk.ikim23.rsswatcher.activity.drawer.CurrentChannelLoader;
import sk.ikim23.rsswatcher.activity.drawer.FeedLoaderCallbacks;
import sk.ikim23.rsswatcher.activity.drawer.RecyclerViewAdapter;
import sk.ikim23.rsswatcher.activity.feed.FeedActivity;
import sk.ikim23.rsswatcher.service.NotificationDeleteService;

public class NotificationResultActivity extends AppCompatActivity implements RecyclerViewAdapter.OnItemClickListener {

    private static final int LOADER_ID = 999;
    private RecyclerView recyclerView;
    private RecyclerViewAdapter recyclerAdapter;
    private FeedLoaderCallbacks feedLoader;
    private boolean isServiceExecuted;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_notification_result);
        Toolbar toolbar = (Toolbar) findViewById(R.id.toolbar);
        setSupportActionBar(toolbar);

        Intent intent = getIntent();
        if (intent != null) {
            ActionBar actionBar = getSupportActionBar();
            if (actionBar != null) {
                actionBar.setTitle(intent.getStringExtra(U.EXTRA_CHANNEL_NAME));
            }
            recyclerView = (RecyclerView) findViewById(R.id.recycler_view);
            recyclerView.setLayoutManager(new LinearLayoutManager(this));
            recyclerView.addItemDecoration(new DividerItemDecoration(this, DividerItemDecoration.VERTICAL));
            // fix separator blinking
            RecyclerView.ItemAnimator animator = recyclerView.getItemAnimator();
            if (animator instanceof SimpleItemAnimator) {
                ((SimpleItemAnimator) animator).setSupportsChangeAnimations(false);
            }
            recyclerAdapter = new RecyclerViewAdapter(this, this);
            recyclerView.setAdapter(recyclerAdapter);
            feedLoader = new FeedLoaderCallbacks(this, recyclerAdapter);
            getSupportLoaderManager().initLoader(LOADER_ID, intent.getExtras(), feedLoader);

            if (!isServiceExecuted) {
                long filterId = intent.getLongExtra(U.EXTRA_FILTER_ID, U.FILTER_ID_DEFAULT);
                if (filterId != U.FILTER_ID_DEFAULT) {
                    Intent serviceIntent = new Intent(this, NotificationDeleteService.class);
                    serviceIntent.putExtra(U.EXTRA_FILTER_ID, filterId);
                    startService(serviceIntent);
                }
                isServiceExecuted = true;
            }
        }
    }

    @Override
    public void onListItemClick(View view, int position, long id) {
        Intent intent = new Intent(this, FeedActivity.class);
        String currentChannelName = CurrentChannelLoader.getInstance().getCurrentChannelName();
        intent.putExtras(feedLoader.getBundle());
        intent.putExtra(U.EXTRA_CHANNEL_NAME, currentChannelName);
        // must be after: intent.putExtras(feedLoader.getBundle()); to rewrite EXTRA_POSITION
        intent.putExtra(U.EXTRA_POSITION, position);
        intent.putExtra(U.EXTRA_SERVICE_EXECUTED, true);
        startActivity(intent);
    }

    @Override
    public boolean onListItemLongClick(View view, int position, long id) {
        return false;
    }
}
