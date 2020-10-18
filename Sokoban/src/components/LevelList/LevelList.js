import React, { Component } from 'react';
import { ListView, Text, TouchableHighlight, View } from 'react-native';
import { styles, listItemUnderlayColor } from './styles';

class LevelList extends Component {

  _renderRow = (level) => {
    return (
      <TouchableHighlight style={styles.listItem} underlayColor={listItemUnderlayColor}
        onPress={() => this.props.onItemPress(level.board)}>
        <Text style={styles.text}>{level.name}</Text>
      </TouchableHighlight>
    );
  }

  _renderSeparator = () => <View style={styles.separator}/>

  render() {
    const ds = new ListView.DataSource({rowHasChanged: (l1, l2) => l1.name !== l2.name});
    const dataSource = ds.cloneWithRows(this.props.levels);
    return (
      <ListView
        dataSource={dataSource}
        renderRow={this._renderRow}
        renderSeparator={this._renderSeparator}
        enableEmptySections={true}
        />
    );
  }

}

export { LevelList };
