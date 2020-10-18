package sk.ikim23.aircarrental.gui.model

import javafx.beans.property.SimpleIntegerProperty
import javafx.beans.property.SimpleStringProperty
import sk.ikim23.aircarrental.sim.format
import sk.ikim23.aircarrental.stat.BusStats

class BusModel {
    val id = SimpleIntegerProperty()
    val position = SimpleStringProperty()
    val actOccupation = SimpleStringProperty()
    val rounds = SimpleIntegerProperty()
    val avgOccupation = SimpleStringProperty()
    val lowOccupation = SimpleStringProperty()
    val uppOccupation = SimpleStringProperty()

    fun update(stats: BusStats) {
        id.set(stats.id)
        position.set(stats.position)
        actOccupation.set("${stats.nPassengers}/${stats.capacity}")
        rounds.set(stats.nRounds)
        avgOccupation.set(stats.avgUsage.format())
        lowOccupation.set(stats.lowUsage.format())
        uppOccupation.set(stats.uppUsage.format())
    }
}