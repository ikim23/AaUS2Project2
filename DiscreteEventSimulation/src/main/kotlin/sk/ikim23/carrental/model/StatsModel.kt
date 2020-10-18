package sk.ikim23.carrental.model

import javafx.application.Platform
import javafx.beans.property.SimpleDoubleProperty
import javafx.beans.property.SimpleIntegerProperty
import sk.ikim23.carrental.core.impl.IStats

open class StatsModel {
    val lowSystemTime = SimpleDoubleProperty()
    val avgSystemTime = SimpleDoubleProperty()
    val uppSystemTime = SimpleDoubleProperty()
    val nUsers = SimpleIntegerProperty()
    val avgRoundTime = SimpleDoubleProperty()
    val nBusRounds = SimpleIntegerProperty()
    val avgBusUsage = SimpleDoubleProperty()
    val avgT1QueueSize = SimpleDoubleProperty()
    val avgT2QueueSize = SimpleDoubleProperty()
    val avgServiceDeskQueueSize = SimpleDoubleProperty()
    val avgServiceDeskUsage = SimpleDoubleProperty()

    open fun update(stats: IStats) {
        Platform.runLater {
            nUsers.set(stats.customerCount())
            lowSystemTime.set(stats.lowSysTime())
            avgSystemTime.set(stats.avgSysTime())
            uppSystemTime.set(stats.uppSysTime())
            nBusRounds.set(stats.roundCount())
            avgRoundTime.set(stats.avgRoundTime())
            avgBusUsage.set(stats.avgBusUsage())
            avgT1QueueSize.set(stats.avgT1QueueSize())
            avgT2QueueSize.set(stats.avgT2QueueSize())
            avgServiceDeskQueueSize.set(stats.avgServiceDeskQueueSize())
            avgServiceDeskUsage.set(stats.avgServiceDeskUsage())
        }
    }

    open fun clear() {
        Platform.runLater {
            nUsers.set(0)
            lowSystemTime.set(0.0)
            avgSystemTime.set(0.0)
            uppSystemTime.set(0.0)
            nBusRounds.set(0)
            avgRoundTime.set(0.0)
            avgBusUsage.set(0.0)
            avgT1QueueSize.set(0.0)
            avgT2QueueSize.set(0.0)
            avgServiceDeskQueueSize.set(0.0)
            avgServiceDeskUsage.set(0.0)
        }
    }
}