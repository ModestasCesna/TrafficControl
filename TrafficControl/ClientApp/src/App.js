import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { Scenarios } from './components/Scenarios';
import { AddScenario } from './components/AddScenario';
import { Simulations } from './components/Simulations';
import { Simulation } from './components/Simulation';

import './custom.css'
import { StartSimulation } from './components/StartSimulation';


export default class App extends Component {
  static displayName = App.name;

  render () {
    return (
      <Layout>
        <Route exact path='/' component={Simulations} />
        <Route path='/scenarios' component={Scenarios} exact={true} />
        <Route path='/scenarios/add' component={AddScenario} />
        <Route path='/simulations' component={Simulations} exact={true} />
        <Route path='/start-simulation' component={StartSimulation} exact={true} />
        <Route path='/simulations/:id' component={Simulation} />
      </Layout>
    );
  }
}
