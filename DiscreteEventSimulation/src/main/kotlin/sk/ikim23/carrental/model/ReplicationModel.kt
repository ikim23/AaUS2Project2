package sk.ikim23.carrental.model

import javafx.application.Platform
import javafx.beans.property.SimpleIntegerProperty
import javafx.beans.property.SimpleObjectProperty
import javafx.beans.property.SimpleStringProperty
import sk.ikim23.carrental.core.impl.ISimListener
import sk.ikim23.carrental.core.impl.IStats
import sk.ikim23.carrental.core.impl.Stats
import sk.ikim23.carrental.formatTime
import tornadofx.*

class ReplicationModel: StatsModel() {
    val systemTime = SimpleStringProperty(formatTime(0.0))
    val nBus = SimpleIntegerProperty()
    val nEmployees = SimpleIntegerProperty()
    val debug = SimpleStringProperty()
    val steps = ISimListener.Step.values().asList().observable()
    val step = SimpleObjectProperty(steps.first())
    val timeStep get() = step.get()

    fun onStep(stats: Stats, nBus: Int, nEmpl: Int) {
        super.update(stats)
        Platform.runLater {
            systemTime.set(formatTime(stats.systemTime()))
            this.nBus.set(nBus)
            nEmployees.set(nEmpl)
            val sb = StringBuilder("Bus:\n")
            for (bus in stats.core.buses) {
                sb.append(bus)
                sb.append("\n")
            }
            sb.append("\n")
            sb.append(stats.core.serviceDesk)
            debug.set(sb.toString())
        }
    }

    override fun clear() {
        super.clear()
        Platform.runLater {
            systemTime.set(formatTime(0.0))
        }
    }
}