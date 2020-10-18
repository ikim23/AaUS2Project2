package sk.ikim23.rsswatcher.activity.drawer;

import android.content.ContentValues;
import android.content.Context;
import android.content.DialogInterface;
import android.database.Cursor;
import android.net.Uri;
import android.support.v4.content.ContextCompat;
import android.support.v7.app.AlertDialog;
import android.support.v7.view.ActionMode;
import android.text.TextUtils;
import android.util.Log;
import android.view.Menu;
import android.view.MenuInflater;
import android.view.MenuItem;
import android.widget.Toast;

import sk.ikim23.rsswatcher.R;
import sk.ikim23.rsswatcher.data.DbHelper;
import sk.ikim23.rsswatcher.data.FeedProvider;

public class RecyclerViewBulkAction implements ActionMode.Callback {

    public interface OnActionModeListener {
        void setActionMode(ActionMode mode);
    }

    private final Context context;
    private final OnActionModeListener listener;
    private final RecyclerViewAdapter adapter;

    public RecyclerViewBulkAction(Context context, OnActionModeListener listener, RecyclerViewAdapter adapter) {
        this.context = context;
        this.listener = listener;
        this.adapter = adapter;
    }

    @Override
    public boolean onCreateActionMode(ActionMode mode, Menu menu) {
        listener.setActionMode(mode);
        MenuInflater inflater = mode.getMenuInflater();
        inflater.inflate(R.menu.activity_drawer_menu_multi_choice, menu);
        return true;
    }

    @Override
    public boolean onPrepareActionMode(ActionMode mode, Menu menu) {
        return false;
    }

    @Override
    public boolean onActionItemClicked(final ActionMode mode, MenuItem item) {
        Cursor cursor;
        switch (item.getItemId()) {
            case R.id.action_select_deselect_all:
                String selectTitle = context.getString(R.string.select_all);
                boolean selectAll = selectTitle.equals(item.getTitle());
                item.setIcon(ContextCompat.getDrawable(context, selectAll ? R.drawable.ic_clear_white_24dp : R.drawable.ic_done_all_white_24dp));
                item.setTitle(selectAll ? R.string.deselect_all : R.string.select_all);
                cursor = adapter.getCursor();
                if (cursor != null) {
                    int position = 0;
                    while (cursor.moveToPosition(position)) {
                        adapter.setItemSelected(position, cursor.getLong(cursor.getColumnIndex(DbHelper.FEED_ID)), selectAll);
                        position++;
                    }
                    mode.setTitle(adapter.getSelectedItemCount() > 0 ? String.valueOf(adapter.getSelectedItemCount()) : null);
                }
                return true;
            case R.id.action_archive_selected:
                if (adapter.getSelectedItemCount() > 0) {
                    AlertDialog.Builder builder = new AlertDialog.Builder(context);
                    builder.setTitle(CurrentChannelLoader.getInstance().getCurrentChannelName());
                    builder.setMessage(R.string.dialog_archive_selected_feeds);
                    builder.setNegativeButton(R.string.cancel, new DialogInterface.OnClickListener() {
                        @Override
                        public void onClick(DialogInterface dialog, int which) {
                            dialog.dismiss();
                        }
                    });
                    builder.setPositiveButton(R.string.OK, new DialogInterface.OnClickListener() {
                        @Override
                        public void onClick(DialogInterface dialog, int which) {
                            Uri uri = Uri.parse("content://" + FeedProvider.AUTHORITY + "/" + FeedProvider.PATH_FEED);
                            String selection = DbHelper.FEED_ID + " IN (" + TextUtils.join(",", adapter.getSelectedItemIds()) + ")";
                            Log.d("archive", selection);
                            ContentValues values = new ContentValues();
                            values.put(DbHelper.FEED_IS_ARCHIVED, 1);
                            context.getContentResolver().update(uri, values, selection, null);
                            mode.finish();
                        }
                    });
                    AlertDialog dialog = builder.create();
                    dialog.show();
                } else {
                    Toast.makeText(context, R.string.no_items_selected, Toast.LENGTH_SHORT).show();
                }
                return true;
            case R.id.action_delete_selected:
                if (adapter.getSelectedItemCount() > 0) {
                    AlertDialog.Builder builder = new AlertDialog.Builder(context);
                    builder.setTitle(CurrentChannelLoader.getInstance().getCurrentChannelName());
                    builder.setMessage(R.string.dialog_delete_selected_feeds);
                    builder.setNegativeButton(R.string.cancel, new DialogInterface.OnClickListener() {
                        @Override
                        public void onClick(DialogInterface dialog, int which) {
                            dialog.dismiss();
                        }
                    });
                    builder.setPositiveButton(R.string.OK, new DialogInterface.OnClickListener() {
                        @Override
                        public void onClick(DialogInterface dialog, int which) {
                            Uri uri = Uri.parse("content://" + FeedProvider.AUTHORITY + "/" + FeedProvider.PATH_FEED);
                            String selection = DbHelper.FEED_ID + " IN (" + TextUtils.join(",", adapter.getSelectedItemIds()) + ")";
                            Log.d("remove", selection);
                            ContentValues values = new ContentValues();
                            values.put(DbHelper.FEED_IS_DELETED, 1);
                            context.getContentResolver().update(uri, values, selection, null);
                            mode.finish();
                        }
                    });
                    AlertDialog dialog = builder.create();
                    dialog.show();
                } else {
                    Toast.makeText(context, R.string.no_items_selected, Toast.LENGTH_SHORT).show();
                }
                return true;
        }
        return false;
    }

    @Override
    public void onDestroyActionMode(ActionMode mode) {
        listener.setActionMode(null);
        adapter.deselectAllItems();
    }
}
