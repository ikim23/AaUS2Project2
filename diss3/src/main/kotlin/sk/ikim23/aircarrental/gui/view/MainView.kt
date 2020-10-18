package sk.ikim23.aircarrental.gui.view

import javafx.geometry.Insets
import javafx.geometry.Orientation
import javafx.geometry.Pos
import javafx.scene.Node
import javafx.scene.control.Label
import javafx.scene.layout.Pane
import javafx.scene.text.Font
import javafx.scene.text.FontWeight
import sk.ikim23.aircarrental.entity.BusType
import sk.ikim23.aircarrental.gui.controller.MainController
import tornadofx.*

class MainView : View("AirCarRental") {
    override val root = borderpane()
    val cWidth = 70.0
    val sWidth = 50.0
    val cPadding = Insets(5.0)
    val cSpacing = 5.0
    val cAlignment = Pos.CENTER_LEFT
    val controller: MainController by inject()
    val replStatsView = StatsView(controller.replModel)
    val globStatsView = StatsView(controller.globModel)
    val debugView = DebugView(controller)
    val graphDataView = GraphDataView(controller)
    val graphView = GraphView(controller.graphModel)

    init {
        root.top {
            vbox {
                hbox {
                    padding = cPadding
                    spacing = cSpacing
                    alignment = cAlignment
                    label("Replications:")
                    textfield {
                        prefWidth = cWidth
                        textProperty().bindBidirectional(controller.nReplications, IntConverter())
                    }
                    label("Employees:")
                    textfield {
                        prefWidth = sWidth
                        textProperty().bindBidirectional(controller.nEmployeesFrom, IntConverter())
                    }
                    label("to")
                    textfield {
                        prefWidth = sWidth
                        textProperty().bindBidirectional(controller.nEmployeesTo, IntConverter())
                    }
                    label("Bus:")
                    textfield {
                        prefWidth = sWidth
                        textProperty().bindBidirectional(controller.nBusesFrom, IntConverter())
                    }
                    label("to")
                    textfield {
                        prefWidth = sWidth
                        textProperty().bindBidirectional(controller.nBusesTo, IntConverter())
                    }
                    label("Bus capacity:")
                    combobox<BusType> {
                        items = controller.busTypes
                        valueProperty().bindBidirectional(controller.busType)
                        cellFormat { text = it.capacity.toString() }
                    }
                    button("Start") {
                        prefWidth = cWidth
                        setOnAction { controller.start() }
                    }
                    button("Resume") {
                        prefWidth = cWidth
                        setOnAction { controller.resume() }
                    }
                    button("Pause") {
                        prefWidth = cWidth
                        setOnAction { controller.pause() }
                    }
                    button("Stop") {
                        prefWidth = cWidth
                        setOnAction { controller.stop() }
                    }
                    checkbox {
                        text = "Max speed"
                        selectedProperty().bindBidirectional(controller.maxSpeed)
                    }
                    label("Interval:")
                    slider(1, 300) { valueProperty().bindBidirectional(controller.interval) }
                    label { textProperty().bind(controller.interval.asString("%.1fs")) }
                    label("Duration:")
                    slider(0.1, 1) { valueProperty().bindBidirectional(controller.duration) }
                    label { textProperty().bind(controller.duration.asString("%.1fs")) }
                }
                separator()
                hbox {
                    padding = cPadding
                    spacing = cSpacing
                    alignment = cAlignment
                    label("Simulation time:")
                    label { textProperty().bind(controller.simulationTime) }
                    separator(Orientation.VERTICAL)
                    label("Replication:")
                    label { textProperty().bind(controller.currentReplication) }
                    separator(Orientation.VERTICAL)
                    label("Employees:")
                    label { textProperty().bind(controller.currentEmployees.asString()) }
                    separator(Orientation.VERTICAL)
                    label("Buses:")
                    label { textProperty().bind(controller.currentBuses.asString()) }
                    setChildrenFont(this)
                }
            }
        }
        root.center {
            tabpane {
                tab("Statistics") {
                    isClosable = false
                    splitpane {
                        vbox {
                            label("Replication statistics:") {
                                setFont(this, FontWeight.BOLD, 18.0)
                                padding = Insets(10.0)
                            }
                            children.add(replStatsView.root)
                        }
                        vbox {
                            label("Global statistics:") {
                                setFont(this, FontWeight.BOLD, 18.0)
                                padding = Insets(10.0)
                            }
                            children.add(globStatsView.root)
                        }
                    }
                }
                tab("Debug") {
                    isClosable = false
                    vbox { children.add(debugView.root) }
                }
                tab("Graph data") {
                    isClosable = false
                    vbox { children.add(graphDataView.root) }
                }
                tab("Graphs") {
                    isClosable = false
                    vbox { children.add(graphView.root) }
                }
            }
        }
        root.bottom {
            vbox {
                separator()
                hbox {
                    padding = cPadding
                    spacing = cSpacing
                    alignment = cAlignment
                    label("Simulation state:")
                    label { textProperty().bind(controller.simState) }
                }
            }
        }
    }
}

fun setChildrenFont(pane: Pane, weight: FontWeight = FontWeight.NORMAL, size: Double = 16.0) {
    pane.children.forEach { setFont(it, weight, size) }
}

fun setFont(node: Node, weight: FontWeight = FontWeight.NORMAL, size: Double = 16.0) {
    if (node is Label) {
        node.font = Font.font("Arial", weight, size)
    }
}