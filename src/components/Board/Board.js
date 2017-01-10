import React, { Component } from 'react';
import { View } from 'react-native';
import { StretchView } from '../StretchView';
import { Field } from '../Field';
import { styles } from './styles';

class Board extends Component {

  render() {
    const stretchParams = {
      rows: this.props.board.length,
      cols: this.props.board[0].length
    };
    return (
      <StretchView {...stretchParams}>
      {
        this.props.board.map(row => {
          return (
            <View style={styles.boardRow}>
              { row.map(char => <Field field={char}/>) }
            </View>
          );
        })
      }
      </StretchView>
    );
  }

}

export { Board };
