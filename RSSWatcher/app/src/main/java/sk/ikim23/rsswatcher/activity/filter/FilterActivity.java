package sk.ikim23.rsswatcher.activity.filter;

import android.content.DialogInterface;
import android.net.Uri;
import android.os.Bundle;
import android.support.v7.app.ActionBar;
import android.support.v7.app.AlertDialog;
import android.support.v7.app.AppCompatActivity;
import android.support.v7.widget.DividerItemDecoration;
import android.support.v7.widget.LinearLayoutManager;
import android.support.v7.widget.RecyclerView;
import android.support.v7.widget.SimpleItemAnimator;
import android.support.v7.widget.Toolbar;
import android.view.Menu;
import android.view.MenuItem;

import sk.ikim23.rsswatcher.R;
import sk.ikim23.rsswatcher.U;
import sk.ikim23.rsswatcher.activity.drawer.CurrentChannelLoader;
import sk.ikim23.rsswatcher.data.DbHelper;
import sk.ikim23.rsswatcher.data.FeedProvider;

public class FilterActivity extends AppCompatActivity implements RecyclerViewAdapter.OnItemClickListener {

    private static final int FILTER_LOADER_ID = 888;
    private RecyclerView recyclerView;
    private RecyclerViewAdapter recyclerAdapter;
    private FilterLoaderCallbacks filterLoader;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_filter);
        initToolbar();
        initRecyclerView();
    }

    @Override
    public void onListItemClick(long id, String query) {
        Bundle bundle = new Bundle();
        bundle.putBoolean(U.EXTRA_UPDATE, true);
        bundle.putLong(U.EXTRA_ID, id);
        bundle.putString(U.EXTRA_SEARCH_QUERY, query);
        AddFilterDialog dialog = new AddFilterDialog();
        dialog.setArguments(bundle);
        dialog.show(getSupportFragmentManager(), "AddFilterDialog");
    }

    @Override
    public void onDeleteButtonClick(final long id, final String query) {
        AlertDialog.Builder builder = new AlertDialog.Builder(this);
        builder.setTitle(query);
        builder.setMessage(R.string.dialog_delete_filter_message);
        builder.setNegativeButton(R.string.cancel, new DialogInterface.OnClickListener() {
            @Override
            public void onClick(DialogInterface dialog, int which) {
                dialog.cancel();
            }
        });
        builder.setPositiveButton(R.string.OK, new DialogInterface.OnClickListener() {
            @Override
            public void onClick(DialogInterface dialog, int which) {
                Uri uri = Uri.parse("content://" + FeedProvider.AUTHORITY + "/" + FeedProvider.PATH_FILTER);
                String where = DbHelper.FILTER_ID + "=" + id;
                getContentResolver().delete(uri, where, null);
            }
        });
        AlertDialog dialog = builder.create();
        dialog.show();
    }

    // ****************************************
    // OPTIONS MENU
    // ****************************************

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        getMenuInflater().inflate(R.menu.activity_filter_menu, menu);
        return true;
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        switch (item.getItemId()) {
            case android.R.id.home:
                onBackPressed();
                return true;
            case R.id.action_add_filter:
                AddFilterDialog dialog = new AddFilterDialog();
                dialog.show(getSupportFragmentManager(), "AddFilterDialog");
                return true;
        }
        return super.onOptionsItemSelected(item);
    }

    // ****************************************
    // SETUP METHODS
    // ****************************************

    private void initRecyclerView() {
        recyclerView = (RecyclerView) findViewById(R.id.recycler_view);
        recyclerView.setLayoutManager(new LinearLayoutManager(this));
        recyclerView.addItemDecoration(new DividerItemDecoration(this, DividerItemDecoration.VERTICAL));
        // fix separator blinking
        RecyclerView.ItemAnimator animator = recyclerView.getItemAnimator();
        if (animator instanceof SimpleItemAnimator) {
            ((SimpleItemAnimator) animator).setSupportsChangeAnimations(false);
        }
        recyclerAdapter = new RecyclerViewAdapter(this);
        recyclerView.setAdapter(recyclerAdapter);
        filterLoader = new FilterLoaderCallbacks(this, recyclerAdapter);
        getSupportLoaderManager().initLoader(FILTER_LOADER_ID, null, filterLoader);
    }

    private void initToolbar() {
        String currentChannelName = CurrentChannelLoader.getInstance().getCurrentChannelName();
        Toolbar toolbar = (Toolbar) findViewById(R.id.toolbar);
        toolbar.setTitle(currentChannelName);
        setSupportActionBar(toolbar);
        ActionBar actionBar = getSupportActionBar();
        if (actionBar != null) {
            actionBar.setDisplayHomeAsUpEnabled(true);
        }
    }

}
