import { StyleSheet } from 'react-native';

const styles = StyleSheet.create({
  toolbar: {
    height: 40,
    flexDirection: 'row',
    backgroundColor: '#f7f7f8',
    alignItems: 'center',
  },
  box: {
    flex: 1,
  },
  boxLeft: {
    alignItems: 'flex-start',
  },
  boxRight: {
    alignItems: 'flex-end',
  },
  button: {
    paddingHorizontal: 10,
    paddingVertical: 5,
  },
  buttonWrapper: {
    flexDirection: 'row',
    alignItems: 'center'
  },
  text: {
    color: '#2c90fc',
    fontSize: 16,
  },
  textTitle: {
    color: 'black',
    textAlign: 'center',
  },
  imageBack: {
    width: 16,
    height: 16,
    tintColor: '#2c90fc',
  },
  separator: {
    height: 1,
    backgroundColor: '#e3e3e7',
  }
});
buttonUnderlayColor = 'transparent';

export { styles, buttonUnderlayColor };
