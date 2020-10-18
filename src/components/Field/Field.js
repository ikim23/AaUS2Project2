import React, { Component } from 'react';
import { View } from 'react-native';
import { BOX, BOX_IN_HOME, HOME, PLAYER, PLAYER_IN_HOME, WALL } from '../../logic/sokoban';
import { styles } from './styles';

const FieldFactory = (props) => {
    switch (props.field) {
      case BOX: return <Box color="#5d4036"/>;
      case BOX_IN_HOME: return <Box color="#ff9800"/>;
      case HOME: return <Home/>;
      case PLAYER: return <Player/>;
      case PLAYER_IN_HOME: return <PlayerInHome/>;
      case WALL: return <Wall/>;
      default: return <Field/>;
    }
}

const Field = (props) => {
    return (
      <View style={[styles.field, props.style]}>
        {props.children}
      </View>
    );
}

const Home = () => <Field style={styles.home}/>;

const Wall = () => <Field style={styles.wall}/>;

const Player = () => {
  return (
    <Field>
      <View style={styles.circle}/>
    </Field>
  );
}

const PlayerInHome = () => {
  return (
    <Field style={styles.home}>
      <View style={styles.circle}/>
    </Field>
  );
}

const Box = (props) => {
  return (
    <Field>
      <View style={[styles.boxOuter, {backgroundColor: props.color}]}>
        <View style={styles.boxInner}/>
      </View>
    </Field>
  );
}

export default FieldFactory;
