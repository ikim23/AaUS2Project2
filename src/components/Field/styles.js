import { Dimensions, StyleSheet } from 'react-native';

const boxContureWidth = 2;
const styles = StyleSheet.create({
  field: {
    flex: 1,
  },
  circle: {
    flex: 1,
    margin: 3,
    backgroundColor: 'red',
    borderRadius: Dimensions.get('window').width / 2,
  },
  home: {
    backgroundColor: '#1fb125',
  },
  wall: {
    backgroundColor: 'black',
  },
  boxOuter: {
    flex: 1,
    borderWidth: boxContureWidth,
    borderColor: 'black',
  },
  boxInner: {
    flex: 1,
    margin: 5,
    borderWidth: boxContureWidth,
    borderColor: 'black',
  }
});

export { styles };
