package sk.ikim23.rsswatcher.activity.filter;

import android.app.Dialog;
import android.content.ContentValues;
import android.content.DialogInterface;
import android.net.Uri;
import android.os.Bundle;
import android.support.annotation.NonNull;
import android.support.v4.app.DialogFragment;
import android.support.v7.app.AlertDialog;
import android.view.LayoutInflater;
import android.view.View;
import android.view.Window;
import android.view.WindowManager;
import android.widget.EditText;

import sk.ikim23.rsswatcher.R;
import sk.ikim23.rsswatcher.U;
import sk.ikim23.rsswatcher.activity.drawer.CurrentChannelLoader;
import sk.ikim23.rsswatcher.data.DbHelper;
import sk.ikim23.rsswatcher.data.FeedProvider;

public class AddFilterDialog extends DialogFragment {

    private boolean update;

    @NonNull
    @Override
    public Dialog onCreateDialog(Bundle savedInstanceState) {
        AlertDialog.Builder builder = new AlertDialog.Builder(getActivity());
        LayoutInflater inflater = getActivity().getLayoutInflater();
        View view = inflater.inflate(R.layout.dialog_add_filter, null);
        final EditText query = (EditText) view.findViewById(R.id.filter_query);
        Bundle args = getArguments();
        if (args != null) {
            update = args.getBoolean(U.EXTRA_UPDATE);
            String queryText = args.getString(U.EXTRA_SEARCH_QUERY);
            if (queryText != null) {
                query.setText(queryText);
                query.setSelection(queryText.length());
            }
        }
        builder
                .setTitle(R.string.dialog_add_filter_title)
                .setView(view)
                .setPositiveButton(update ? R.string.dialog_add_filter_btn_update : R.string.add, new DialogInterface.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialog, int which) {
                    }
                })
                .setNegativeButton(R.string.cancel, new DialogInterface.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialog, int which) {
                        dialog.dismiss();
                    }
                });
        final AlertDialog dialog = builder.create();
        dialog.show();
        dialog.getButton(AlertDialog.BUTTON_POSITIVE).setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                String queryText = query.getText().toString();
                if (queryText.length() == 0) {
                    query.setError(getString(R.string.dialog_add_filter_invelid_query));
                    return;
                }
                processInput(queryText);
                dialog.dismiss();
            }
        });
        return dialog;
    }

    @Override
    public void onActivityCreated(Bundle savedInstanceState) {
        super.onActivityCreated(savedInstanceState);
        Window window = getDialog().getWindow();
        if (window != null) {
            window.setSoftInputMode(WindowManager.LayoutParams.SOFT_INPUT_STATE_ALWAYS_VISIBLE);
        }
    }

    private void processInput(String query) {
        Uri uri = Uri.parse("content://" + FeedProvider.AUTHORITY + "/" + FeedProvider.PATH_FILTER);
        ContentValues values = new ContentValues();
        values.put(DbHelper.FILTER_MATCH_QUERY, query);
        if (update) {
            String where = DbHelper.FILTER_ID + "=" + getArguments().getLong(U.EXTRA_ID);
            getContext().getContentResolver().update(uri, values, where, null);
        } else {
            values.put(DbHelper.FILTER_CHANNEL_ID, CurrentChannelLoader.getInstance().getCurrentChannelId());
            getContext().getContentResolver().insert(uri, values);
        }
    }

}
