import React, { Component } from 'react';
import { Image, TouchableHighlight, View, StyleSheet } from 'react-native';
import images from '../../img';
import { styles, buttonUnderlayColor } from './styles';

class Navigation extends Component {

  render() {
    return (
      <View style={styles.navWrapper}>
        <TouchableHighlight onPress={() => this.props.onMove([0, -1])}
          underlayColor={buttonUnderlayColor}>
          <Image style={styles.image} source={images.arrowUp}/>
        </TouchableHighlight>
        <View style={styles.wrapper}>
          <TouchableHighlight onPress={() => this.props.onMove([-1, 0])}
            underlayColor={buttonUnderlayColor}>
            <Image style={styles.image} source={images.arrowLeft}/>
          </TouchableHighlight>
          <View style={styles.image}/>
          <TouchableHighlight onPress={() => this.props.onMove([1, 0])}
            underlayColor={buttonUnderlayColor}>
            <Image style={styles.image} source={images.arrowRight}/>
          </TouchableHighlight>
        </View>
        <TouchableHighlight onPress={() => this.props.onMove([0, 1])}
          underlayColor={buttonUnderlayColor}>
          <Image style={styles.image} source={images.arrowDown}/>
        </TouchableHighlight>
      </View>
    );
  }

}

export { Navigation };
