import React, { Component } from 'react';
import { Image, Text, TouchableHighlight, View } from 'react-native';
import images from '../../img';
import { styles, buttonUnderlayColor } from './styles';

const title = 'Sokoban';
const btnBack = 'Back';
const btnUndo = 'Undo';

const ToolbarSimple = () => {
  return (
    <View>
      <View style={styles.toolbar}>
        <Text style={[styles.box, styles.text, styles.textTitle]}>
          {title}
        </Text>
      </View>
      <View style={styles.separator}/>
    </View>
  );
}

const Toolbar = (props) => {
  return (
    <View>
      <View style={styles.toolbar}>
        <View style={[styles.box, styles.boxLeft]}>
          <TouchableHighlight style={styles.button} onPress={props.onBack} underlayColor={buttonUnderlayColor}>
            <View style={styles.buttonWrapper}>
              <Image style={styles.imageBack} source={images.back}/>
              <Text style={[styles.text, styles.textBack]}>
                {btnBack}
              </Text>
            </View>
          </TouchableHighlight>
        </View>
        <Text style={[styles.box, styles.text, styles.textTitle]}>
          {title}
        </Text>
        <View style={[styles.box, styles.boxRight]}>
          <TouchableHighlight style={styles.button} onPress={props.onUndo} underlayColor={buttonUnderlayColor}>
            <Text style={styles.text}>
              {btnUndo}
            </Text>
          </TouchableHighlight>
        </View>
      </View>
    <View style={styles.separator}/>
    </View>
  );
}

export { ToolbarSimple, Toolbar };
