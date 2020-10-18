package sk.ikim23.carrental.model

import javafx.application.Platform
import javafx.beans.property.SimpleIntegerProperty
import sk.ikim23.carrental.core.impl.IStats
import sk.ikim23.carrental.core.impl.Stats
import sk.ikim23.carrental.safeDiv

class SummaryModel : StatsModel(), IStats {
    val nReplications = SimpleIntegerProperty()
    var nCustomers = 0
    var sumCustomerTime = 0.0
    var sumCustomerTimeSquare = 0.0
    var nRounds = 0
    var sumRoundTime = 0.0
    var sumAvgBusUsage = 0.0
    var sumAvgT1Size = 0.0
    var sumAvgT2Size = 0.0
    var sumAvgServiceDeskSize = 0.0
    var sumAvgServiceDeskUsage = 0.0

    fun take(stats: Stats) {
        nCustomers += stats.nCustomers
        sumCustomerTime += stats.sumTime
        sumCustomerTimeSquare += stats.sumTimeSquare
        nRounds += stats.nBuses
        sumRoundTime += stats.sumRoundTime
        sumAvgBusUsage += stats.avgBusUsage()
        sumAvgT1Size += stats.avgT1QueueSize()
        sumAvgT2Size += stats.avgT2QueueSize()
        sumAvgServiceDeskSize += stats.avgServiceDeskQueueSize()
        sumAvgServiceDeskUsage += stats.avgServiceDeskUsage()
        Platform.runLater {
            nReplications.set(nReplications.get() + 1)
        }
        super.update(this)
    }

    override fun clear() {
        nCustomers = 0
        sumCustomerTime = 0.0
        sumCustomerTimeSquare = 0.0
        nRounds = 0
        sumRoundTime = 0.0
        sumAvgBusUsage = 0.0
        sumAvgT1Size = 0.0
        sumAvgT2Size = 0.0
        sumAvgServiceDeskSize = 0.0
        sumAvgServiceDeskUsage = 0.0
        super.clear()
        Platform.runLater {
            nReplications.set(0)
        }
    }

    override fun systemTime() = 0.0
    override fun customerCount() = nCustomers
    override fun avgSysTime() = (sumCustomerTime / nCustomers) safeDiv 60
    override fun lowSysTime(): Double {
        val w = sumCustomerTime / nCustomers
        val s = Math.sqrt(sumCustomerTimeSquare / nCustomers - w * w)
        val avg = sumCustomerTime / nCustomers
        return avg - 1.96 * s / Math.sqrt((nCustomers - 1).toDouble()) safeDiv 60
    }

    override fun uppSysTime(): Double {
        val w = sumCustomerTime / nCustomers
        val s = Math.sqrt(sumCustomerTimeSquare / nCustomers - w * w)
        val avg = sumCustomerTime / nCustomers
        return avg + 1.96 * s / Math.sqrt((nCustomers - 1).toDouble()) safeDiv 60
    }

    override fun roundCount() = nRounds
    override fun avgRoundTime() = (sumRoundTime / nRounds) safeDiv 60
    override fun avgBusUsage() = sumAvgBusUsage safeDiv nReplications.get()
    override fun avgT1QueueSize() = sumAvgT1Size safeDiv nReplications.get()
    override fun avgT2QueueSize() = sumAvgT2Size safeDiv nReplications.get()
    override fun avgServiceDeskQueueSize() = sumAvgServiceDeskSize safeDiv nReplications.get()
    override fun avgServiceDeskUsage() = sumAvgServiceDeskUsage safeDiv nReplications.get()
}