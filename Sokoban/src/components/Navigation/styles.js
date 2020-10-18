import { StyleSheet } from 'react-native';

const side = 40;
const buttonUnderlayColor = 'transparent';
const styles = StyleSheet.create({
  navWrapper: {
    height: side * 3,
    justifyContent: 'center',
    alignItems: 'center'
  },
  wrapper: {
    flexDirection: 'row',
    justifyContent: 'center',

  },
  image: {
    width: side,
    height: side,
  }
});

export { styles, buttonUnderlayColor };
