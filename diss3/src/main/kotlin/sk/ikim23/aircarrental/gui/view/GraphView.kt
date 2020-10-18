package sk.ikim23.aircarrental.gui.view

import javafx.scene.chart.NumberAxis
import javafx.scene.layout.Priority
import sk.ikim23.aircarrental.gui.model.GraphModel
import tornadofx.*

class GraphView(model: GraphModel) : View() {
    override val root = vbox()

    init {
        root.hgrow = Priority.ALWAYS
        root.vgrow = Priority.ALWAYS
        hbox {
            linechart("# Buses [arriving passenger]", NumberAxis(), NumberAxis()) {
                (xAxis as NumberAxis).forceZeroInRangeProperty().set(false)
                (yAxis as NumberAxis).forceZeroInRangeProperty().set(false)
                animated = false
                data = model.arrivingPassengersBusChartData
                hgrow = Priority.ALWAYS
                vgrow = Priority.ALWAYS
            }
            linechart("# Buses [leaving passenger]", NumberAxis(), NumberAxis()) {
                (xAxis as NumberAxis).forceZeroInRangeProperty().set(false)
                (yAxis as NumberAxis).forceZeroInRangeProperty().set(false)
                animated = false
                data = model.leavingPassengersBusChartData
                hgrow = Priority.ALWAYS
                vgrow = Priority.ALWAYS
            }
            hgrow = Priority.ALWAYS
            vgrow = Priority.ALWAYS
        }
        hbox {
            linechart("# Employees [arriving passenger]", NumberAxis(), NumberAxis()) {
                (xAxis as NumberAxis).forceZeroInRangeProperty().set(false)
                (yAxis as NumberAxis).forceZeroInRangeProperty().set(false)
                animated = false
                data = model.arrivingPassengersEmplChartData
                hgrow = Priority.ALWAYS
                vgrow = Priority.ALWAYS
            }
            linechart("# Employees [leaving passenger]", NumberAxis(), NumberAxis()) {
                (xAxis as NumberAxis).forceZeroInRangeProperty().set(false)
                (yAxis as NumberAxis).forceZeroInRangeProperty().set(false)
                animated = false
                data = model.leavingPassengersEmplChartData
                hgrow = Priority.ALWAYS
                vgrow = Priority.ALWAYS
            }
            hgrow = Priority.ALWAYS
            vgrow = Priority.ALWAYS
        }
    }
}