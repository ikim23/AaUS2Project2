import React, { Component } from 'react';
import { BackAndroid, Text, View, Navigator } from 'react-native';
import { route as levelRoute } from '../../scenes/LevelScene';

class SceneNavigator extends Component {

  renderScene = (route, navigator) => {
    BackAndroid.addEventListener('hardwareBackPress', () => {
      if (route.id !== levelRoute.id) {
          navigator.pop();
          return true;
      }
      BackAndroid.exitApp();
    });
    return <route.component {...{route, navigator}}/>;
  }

  render() {
    return (
      <Navigator
        style={{flex: 1}}
        initialRoute={levelRoute}
        renderScene={this.renderScene}
        configureScene={(route, routeStack) => Navigator.SceneConfigs.FadeAndroid}
      />
    );
  }

}

export { SceneNavigator };
