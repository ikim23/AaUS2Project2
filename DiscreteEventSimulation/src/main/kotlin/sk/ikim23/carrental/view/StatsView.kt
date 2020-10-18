package sk.ikim23.carrental.view

import javafx.scene.control.Label
import javafx.scene.text.Font
import sk.ikim23.carrental.model.StatsModel
import tornadofx.*

class StatsView(var model: StatsModel) : View() {
    override val root = gridpane()

    init {
        root.hgap = 5.0
        root.vgap = 5.0
        root.row {
            label("Users:")
            label { textProperty().bind(model.nUsers.asString()) }
        }
        root.row {
            label("Low user time:")
            label { textProperty().bind(model.lowSystemTime.asString()) }
        }
        root.row {
            label("Avg user time:")
            label { textProperty().bind(model.avgSystemTime.asString()) }
        }
        root.row {
            label("Upp user time:")
            label { textProperty().bind(model.uppSystemTime.asString()) }
        }
        root.row {
            label("Bus rounds:")
            label { textProperty().bind(model.nBusRounds.asString()) }
        }
        root.row {
            label("Bus round time:")
            label { textProperty().bind(model.avgRoundTime.asString()) }
        }
        root.row {
            label("Bus usage:")
            label { textProperty().bind(model.avgBusUsage.asString()) }
        }
        root.row {
            label("T1 queue length:")
            label { textProperty().bind(model.avgT1QueueSize.asString()) }
        }
        root.row {
            label("T2 queue length:")
            label { textProperty().bind(model.avgT2QueueSize.asString()) }
        }
        root.row {
            label("Service desk queue length:")
            label { textProperty().bind(model.avgServiceDeskQueueSize.asString()) }
        }
        root.row {
            label("Service desk usage:")
            label { textProperty().bind(model.avgServiceDeskUsage.asString()) }
        }
        root.children.forEach { if (it is Label) it.font = Font("Arial", 16.0) }
    }
}