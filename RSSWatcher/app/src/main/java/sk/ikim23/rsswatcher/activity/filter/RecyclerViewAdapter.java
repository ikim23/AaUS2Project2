package sk.ikim23.rsswatcher.activity.filter;

import android.database.Cursor;
import android.support.v7.widget.RecyclerView;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.TextView;

import sk.ikim23.rsswatcher.R;
import sk.ikim23.rsswatcher.activity.drawer.SwapableAdapter;
import sk.ikim23.rsswatcher.data.DbHelper;

public class RecyclerViewAdapter extends RecyclerView.Adapter<RecyclerViewAdapter.ViewHolder> implements SwapableAdapter {

    public interface OnItemClickListener {
        void onListItemClick(long id, String query);

        void onDeleteButtonClick(long id, String query);
    }

    private final OnItemClickListener listener;
    private Cursor cursor;

    public RecyclerViewAdapter(OnItemClickListener listener) {
        this.listener = listener;
    }

    @Override
    public ViewHolder onCreateViewHolder(ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.list_item_filter, parent, false);
        return new ViewHolder(view, listener);
    }

    @Override
    public void onBindViewHolder(ViewHolder holder, int position) {
        if (cursor.moveToPosition(position)) {
            holder.itemId = cursor.getLong(cursor.getColumnIndex(DbHelper.FILTER_ID));
            holder.filter.setText(cursor.getString(cursor.getColumnIndex(DbHelper.FILTER_MATCH_QUERY)));
        }
    }

    @Override
    public int getItemCount() {
        return cursor != null ? cursor.getCount() : 0;
    }

    @Override
    public void swapCursor(Cursor newCursor) {
        if (cursor != null) {
            cursor.close();
        }
        cursor = newCursor;
        notifyDataSetChanged();
    }

    static class ViewHolder extends RecyclerView.ViewHolder {

        long itemId = -1L;
        final TextView filter;
        final ImageView btnDelete;

        public ViewHolder(View view, final OnItemClickListener listener) {
            super(view);
            filter = (TextView) view.findViewById(R.id.filter_text);
            btnDelete = (ImageView) view.findViewById(R.id.btn_delete);
            view.setOnClickListener(new View.OnClickListener() {
                @Override
                public void onClick(View view) {
                    listener.onListItemClick(itemId, filter.getText().toString());
                }
            });
            btnDelete.setOnClickListener(new View.OnClickListener() {
                @Override
                public void onClick(View view) {
                    listener.onDeleteButtonClick(itemId, filter.getText().toString());
                }
            });
        }
    }
}
