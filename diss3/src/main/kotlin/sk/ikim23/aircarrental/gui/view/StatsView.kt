package sk.ikim23.aircarrental.gui.view

import javafx.geometry.Insets
import javafx.geometry.Pos
import javafx.scene.control.Label
import javafx.scene.layout.HBox
import javafx.scene.text.FontWeight
import sk.ikim23.aircarrental.gui.model.ReplicationStatsModel
import tornadofx.*

class StatsView(val model: ReplicationStatsModel) : View() {
    override val root = vbox()

    init {
        root.padding = Insets(10.0)
        root.spacing = 5.0
        root.alignment = Pos.CENTER_LEFT
        hbox {
            label("System time:")
            label { textProperty().bind(model.time) }
        }
        label("Passenger count:")
        hbox {
            label("T1:")
            label { textProperty().bind(model.t1ArrivalCount) }
        }
        hbox {
            label("T2:")
            label { textProperty().bind(model.t2ArrivalCount) }
        }
        hbox {
            label("Returning:")
            label { textProperty().bind(model.serviceDeskArrivalCount) }
        }
        label("Arriving passenger system time:")
        hbox {
            label("Average:")
            label { textProperty().bind(model.avgSystemTimeArrivingPassenger) }
        }
        hbox { label { textProperty().bind(model.ciSystemTimeArrivingPassenger) } }
        label("Leaving passenger system time:")
        hbox {
            label("Average:")
            label { textProperty().bind(model.avgSystemTimeLeavingPassenger) }
        }
        hbox { label { textProperty().bind(model.ciwSystemTimeLeavingPassenger) } }
        label("T1 queue:")
        hbox {
            label("Average:")
            label { textProperty().bind(model.avgQueueSizeT1) }
        }
        hbox { label { textProperty().bind(model.ciQueueSizeT1) } }
        label("T2 queue:")
        hbox {
            label("Average:")
            label { textProperty().bind(model.avgQueueSizeT2) }
        }
        hbox { label { textProperty().bind(model.ciQueueSizeT2) } }
        label("Queue before service:")
        hbox {
            label("Average:")
            label { textProperty().bind(model.avgQueueSizeService) }
        }
        hbox { label { textProperty().bind(model.ciQueueSizeService) } }
        label("Queue for bus to T3:")
        hbox {
            label("Average:")
            label { textProperty().bind(model.avgQueueSizeToT3) }
        }
        hbox { label { textProperty().bind(model.ciQueueSizeToT3) } }
        label("Costs:")
        hbox {
            label("Driven km:")
            label { textProperty().bind(model.drivenKm) }
        }
        hbox {
            label("Fuel:")
            label { textProperty().bind(model.driveCosts) }
        }
        hbox {
            label("Drivers:")
            label { textProperty().bind(model.driverCosts) }
        }
        hbox {
            label("Employees:")
            label { textProperty().bind(model.employeeCosts) }
        }
        hbox {
            label("Total:")
            label { textProperty().bind(model.totalCosts) }
        }
        root.children.forEach {
            if (it is HBox) {
                setChildrenFont(it)
                it.spacing = 5.0
            } else if (it is Label) {
                setFont(it, FontWeight.BOLD)
            }
        }
    }
}