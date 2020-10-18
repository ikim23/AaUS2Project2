package sk.ikim23.rsswatcher.activity.drawer;

import android.content.ContentValues;
import android.content.Context;
import android.database.Cursor;
import android.net.Uri;
import android.support.v4.content.ContextCompat;
import android.support.v7.widget.RecyclerView;
import android.text.format.DateUtils;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.TextView;

import java.util.Date;
import java.util.HashSet;

import sk.ikim23.rsswatcher.R;
import sk.ikim23.rsswatcher.U;
import sk.ikim23.rsswatcher.data.DbHelper;
import sk.ikim23.rsswatcher.data.FeedProvider;

public class RecyclerViewAdapter extends RecyclerView.Adapter<RecyclerViewAdapter.ViewHolder> implements SwapableAdapter {

    public interface OnItemClickListener {
        void onListItemClick(View view, int position, long id);

        boolean onListItemLongClick(View view, int position, long id);
    }

    private final HashSet<Long> selectedItemIds = new HashSet<>();
    private final OnItemClickListener listener;
    private final Context context;
    private Cursor cursor;
    private final View.OnClickListener starListener = new View.OnClickListener() {
        @Override
        public void onClick(View view) {
            long itemId = (long) view.getTag(R.id.item_id);
            int position = (int) view.getTag(R.id.item_position);
            boolean isArchived = (boolean) view.getTag(R.id.item_is_archived);
            Uri uri = Uri.parse("content://" + FeedProvider.AUTHORITY + "/" + FeedProvider.PATH_FEED + "/" + itemId);
            ContentValues values = new ContentValues();
            values.put(DbHelper.FEED_IS_ARCHIVED, isArchived ? 0 : 1);
            context.getContentResolver().update(uri, values, null, null);
            notifyItemChanged(position);
        }
    };

    public RecyclerViewAdapter(OnItemClickListener listener, Context context) {
        this.listener = listener;
        this.context = context;
    }

    @Override
    public ViewHolder onCreateViewHolder(ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.list_item_feed, parent, false);
        return new ViewHolder(view, listener);
    }

    @Override
    public void onBindViewHolder(ViewHolder holder, int position) {
        if (cursor.moveToPosition(position)) {
            holder.itemId = cursor.getLong(cursor.getColumnIndex(DbHelper.FEED_ID));
            boolean selected = selectedItemIds.contains(holder.itemId);
            holder.itemView.setSelected(selected);
            holder.title.setText(cursor.getString(cursor.getColumnIndex(DbHelper.FEED_TITLE)));
            holder.title.setTextColor(ContextCompat.getColor(context, selected ? R.color.white : cursor.getInt(cursor.getColumnIndex(DbHelper.FEED_IS_READ)) == 0 ? R.color.black : R.color.gray));
            String descriptionNoHtml = cursor.getString(cursor.getColumnIndex(DbHelper.FEED_DESCRIPTION_NO_HTML));
            if (descriptionNoHtml != null) {
                holder.description.setText(descriptionNoHtml);
                holder.description.setTextColor(ContextCompat.getColor(context, selected ? R.color.white_alfa : R.color.gray));
                holder.description.setVisibility(View.VISIBLE);
            } else {
                holder.description.setVisibility(View.GONE);
            }
            long pubDate = cursor.getLong(cursor.getColumnIndex(DbHelper.FEED_PUB_DATE));
            holder.pubDate.setText(DateUtils.isToday(pubDate) ? U.HH_mm.format(new Date(pubDate)) : U.MMM_d.format(new Date(pubDate)));
            holder.pubDate.setTextColor(ContextCompat.getColor(context, selected ? R.color.white_alfa : R.color.gray));
            boolean archived = cursor.getInt(cursor.getColumnIndex(DbHelper.FEED_IS_ARCHIVED)) != 0;
            holder.star.setImageResource(archived ? R.drawable.ic_star_white_24dp : R.drawable.ic_star_border_white_24dp);
            holder.star.setColorFilter(ContextCompat.getColor(context, selected ? R.color.white_alfa : archived ? R.color.gold : R.color.gray));
            holder.star.setTag(R.id.item_id, holder.itemId);
            holder.star.setTag(R.id.item_position, position);
            holder.star.setTag(R.id.item_is_archived, archived);
            holder.star.setOnClickListener(starListener);
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

    public Cursor getCursor() {
        return cursor;
    }

    public boolean isItemSelected(Long id) {
        return selectedItemIds.contains(id);
    }

    public void setItemSelected(int position, Long id, boolean selected) {
        Log.d("set selected item", "pos:" + position + " id: " + id + " sel: " + selected);
        boolean updateView = selected ? selectedItemIds.add(id) : selectedItemIds.remove(id);
        if (updateView) {
            Log.d("set selected item", "updating");
            notifyItemChanged(position);
        }
    }

    public void deselectAllItems() {
        selectedItemIds.clear();
        notifyDataSetChanged();
    }

    public int getSelectedItemCount() {
        return selectedItemIds.size();
    }

    public HashSet<Long> getSelectedItemIds() {
        return selectedItemIds;
    }

    static class ViewHolder extends RecyclerView.ViewHolder implements View.OnClickListener, View.OnLongClickListener {

        long itemId = -1L;
        final TextView title;
        final TextView description;
        final TextView pubDate;
        final ImageView star;
        final OnItemClickListener listener;

        public ViewHolder(View view, OnItemClickListener listener) {
            super(view);
            view.setOnClickListener(this);
            view.setOnLongClickListener(this);
            title = (TextView) view.findViewById(R.id.title);
            description = (TextView) view.findViewById(R.id.description);
            pubDate = (TextView) view.findViewById(R.id.pub_date);
            star = (ImageView) view.findViewById(R.id.star);
            this.listener = listener;
        }

        @Override
        public void onClick(View view) {
            listener.onListItemClick(view, getAdapterPosition(), itemId);
        }

        @Override
        public boolean onLongClick(View view) {
            return listener.onListItemLongClick(view, getAdapterPosition(), itemId);
        }
    }
}
