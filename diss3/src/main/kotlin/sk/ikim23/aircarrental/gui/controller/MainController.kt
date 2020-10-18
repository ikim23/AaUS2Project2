package sk.ikim23.aircarrental.gui.controller

import OSPABA.ISimDelegate
import OSPABA.SimState
import OSPABA.Simulation
import javafx.application.Platform
import javafx.beans.property.*
import javafx.collections.FXCollections
import javafx.collections.ObservableList
import sk.ikim23.aircarrental.entity.BusType
import sk.ikim23.aircarrental.gui.model.*
import sk.ikim23.aircarrental.sim.MySimulation
import sk.ikim23.aircarrental.sim.Config
import sk.ikim23.aircarrental.sim.toTime
import tornadofx.*

class MainController : Controller(), ISimDelegate {
    val sim = MySimulation()

    val nReplications = SimpleIntegerProperty(1000)
    val nEmployeesFrom = SimpleIntegerProperty(2)
    val nEmployeesTo = SimpleIntegerProperty(nEmployeesFrom.get() + 1)
    val nBusesFrom = SimpleIntegerProperty(2)
    val nBusesTo = SimpleIntegerProperty(nBusesFrom.get() + 1)
    val busTypes = BusType.values().asList().observable()
    val busType = SimpleObjectProperty(busTypes.first())
    val maxSpeed = SimpleBooleanProperty()
    val interval = SimpleDoubleProperty(150.0)
    val duration = SimpleDoubleProperty(.1)

    val simulationTime = SimpleStringProperty(.0.toTime())
    val currentReplication = SimpleStringProperty()
    val currentEmployees = SimpleIntegerProperty()
    val currentBuses = SimpleIntegerProperty()
    val simState = SimpleStringProperty()
    var stopped = false

    val replModel = ReplicationStatsModel()
    val globModel = ReplicationStatsModel()
    val buses = mutableListOf<BusModel>().observable()
    val serviceDesks = mutableListOf<ServiceDeskModel>().observable()
    val simConfigs = mutableListOf<GraphDataModel>().observable()
    val graphModel = GraphModel()

    init {
        maxSpeed.onChange { updateSimSpeed() }
        interval.onChange { updateSimSpeed() }
        duration.onChange { updateSimSpeed() }
        sim.registerDelegate(this)
        sim.onSimulationWillStart {
            Platform.runLater {
                simulationTime.set(.0.toTime())
                currentReplication.set("${sim.currentReplication() + 1}/${sim.replicationCount()}")
                initDebugModel()
            }
        }
        sim.onSimulationDidFinish {
            Platform.runLater {
                val graphData = GraphDataModel(currentEmployees.get(), currentBuses.get(), sim.globStats())
                simConfigs.add(graphData)
                graphModel.take(graphData)
                if (!stopped) {
                    currentBuses.set(currentBuses.get() + 1)
                    if (currentBuses.get() > nBusesTo.get()) {
                        currentBuses.set(nBusesFrom.get())
                        currentEmployees.set(currentEmployees.get() + 1)
                    }
                    if (currentEmployees.get() > nEmployeesTo.get()) {
                        currentBuses.set(nBusesTo.get())
                        currentEmployees.set(nEmployeesTo.get())
                    } else {
                        sim.configure(currentEmployees.get(), currentBuses.get(), busType.get(), Config.stopArrivalsAtTime)
                        sim.simulateAsync(nReplications.get())
                    }
                }
            }
        }
        sim.onReplicationWillStart { updateSimSpeed() }
        sim.onReplicationDidFinish {
            Platform.runLater {
                currentReplication.set("${sim.currentReplication() + 1}/${sim.replicationCount()}")
                globModel.update(sim.globStats())
            }
        }
    }

    override fun refresh(s: Simulation) {
        Platform.runLater {
            val replStats = sim.replStats()
            simulationTime.set(sim.currentTime().toTime())
            replModel.update(replStats)
            replStats.busStats.forEachIndexed { i, stats ->
                buses[i].update(stats)
            }
            replStats.serviceDeskStats.forEachIndexed { i, stats ->
                serviceDesks[i].update(stats)
            }
        }
    }

    override fun simStateChanged(s: Simulation, state: SimState) {
        Platform.runLater { simState.set(state.toString()) }
    }

    fun start() {
        stopped = false
        simConfigs.clear()
        graphModel.clear()
        currentEmployees.set(nEmployeesFrom.get())
        currentBuses.set(nBusesFrom.get())
        sim.configure(currentEmployees.get(), currentBuses.get(), busType.get(), Config.stopArrivalsAtTime)
        sim.simulateAsync(nReplications.get())
    }

    fun resume() {
        sim.resumeSimulation()
    }

    fun pause() {
        sim.pauseSimulation()
    }

    fun stop() {
        stopped = true
        sim.stopSimulation()
    }

    private fun updateSimSpeed() {
        if (maxSpeed.get()) {
            sim.setMaxSimSpeed()
        } else {
            sim.setSimSpeed(interval.get(), duration.get())
        }
    }

    private fun initDebugModel() {
        buses.clear()
        buses.addAll(observableList(currentBuses.get()) { BusModel() })
        serviceDesks.clear()
        serviceDesks.addAll(observableList(currentEmployees.get()) { ServiceDeskModel() })
    }

    private fun <T> observableList(size: Int, createElement: (i: Int) -> T): ObservableList<T> {
        val list = FXCollections.observableArrayList<T>()
        for (i in 0 until size) {
            list.add(createElement(i))
        }
        return list
    }
}