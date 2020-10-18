import React from 'react';
import { Provider } from 'react-redux';
import { SceneNavigator } from './components/SceneNavigator';
import store from './store/store';

export default App = () => {
  return (
    <Provider store={store}>
      <SceneNavigator/>
    </Provider>
  );
}
