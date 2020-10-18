package sk.ikim23.aircarrental.gui.model

import javafx.beans.property.SimpleIntegerProperty
import javafx.beans.property.SimpleStringProperty
import sk.ikim23.aircarrental.sim.format
import sk.ikim23.aircarrental.stat.ServiceDeskStats

class ServiceDeskModel {
    val id = SimpleIntegerProperty()
    val passenger = SimpleStringProperty()
    val avgUsage = SimpleStringProperty()
    val lowBound = SimpleStringProperty()
    val uppBound = SimpleStringProperty()

    fun update(stats: ServiceDeskStats) {
        id.set(stats.id)
        passenger.set(stats.passenger)
        avgUsage.set(stats.avgUsage.format())
        lowBound.set(stats.lowUsage.format())
        uppBound.set(stats.uppUsage.format())
    }
}