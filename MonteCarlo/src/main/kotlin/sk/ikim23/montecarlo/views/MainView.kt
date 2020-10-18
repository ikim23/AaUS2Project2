package sk.ikim23.montecarlo.views

import javafx.geometry.Insets
import javafx.geometry.Orientation
import javafx.geometry.Pos
import javafx.scene.chart.NumberAxis
import sk.ikim23.montecarlo.controllers.MainController
import tornadofx.*

class MainView : View() {
    override val root = borderpane()
    val cWidth = 70.0
    val cPadding = Insets(5.0)
    val cSpacing = 5.0
    val cAlignment = Pos.CENTER_LEFT
    val controller: MainController by inject()

    init {
        title = "Monte Carlo"
        root.top {
            vbox {
                hbox {
                    padding = cPadding
                    spacing = cSpacing
                    alignment = cAlignment
                    label("Replications:")
                    textfield {
                        prefWidth = cWidth
                        textProperty().bindBidirectional(controller.render.replicationsProperty, IntConverter())
                        disableProperty().bind(controller.simRunningProperty)
                    }
                    label("Points/Group:")
                    textfield {
                        prefWidth = cWidth
                        textProperty().bindBidirectional(controller.render.maxPointsProperty, IntConverter())
                        disableProperty().bind(controller.simRunningProperty)
                    }
                    button("Start") {
                        prefWidth = cWidth
                        disableProperty().bind(controller.render.startDisableProperty())
                        setOnAction { controller.start() }
                    }
                    button("Pause") {
                        prefWidth = cWidth
                        disableProperty().bind(controller.render.pauseDisableProperty())
                        setOnAction { controller.pause() }
                    }
                    button("Stop") {
                        prefWidth = cWidth
                        disableProperty().bind(controller.render.stopDisableProperty())
                        setOnAction { controller.stop() }
                    }
                }
                hbox {
                    padding = cPadding
                    spacing = cSpacing
                    alignment = cAlignment
                    label("Doors:")
                    textfield {
                        prefWidth = 50.0
                        textProperty().bindBidirectional(controller.graph.doorsProperty, IntConverter())
                        disableProperty().bind(controller.simRunningProperty)
                    }
                    checkbox("Show Keep Guess") {
                        selectedProperty().bindBidirectional(controller.graph.keepGuessVisibleProperty)
                        disableProperty().bind(controller.simRunningProperty)
                    }
                    checkbox("Show Change Guess") {
                        selectedProperty().bindBidirectional(controller.graph.changeGuessVisibleProperty)
                        disableProperty().bind(controller.simRunningProperty)
                    }
                }
                separator()
            }
        }
        root.center {
            linechart("", NumberAxis(), NumberAxis()) {
                xAxis.animated = false
                yAxis.animated = false
                animated = false
                data = controller.chartData
            }
        }
        root.bottom {
            vbox {
                separator()
                hbox {
                    padding = cPadding
                    spacing = cSpacing
                    alignment = cAlignment
                    val valueWidth = 120.0
                    label("Keep Guess:")
                    label {
                        prefWidth = valueWidth
                        textProperty().bind(controller.keepGuessValueProperty)
                    }
                    separator(Orientation.VERTICAL)
                    label("Change Guess:")
                    label {
                        prefWidth = valueWidth
                        textProperty().bind(controller.changeGuessValueProperty)
                    }
                }
            }
        }
    }
}