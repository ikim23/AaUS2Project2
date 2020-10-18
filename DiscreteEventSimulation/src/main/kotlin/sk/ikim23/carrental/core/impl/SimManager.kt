package sk.ikim23.carrental.core.impl

import sk.ikim23.carrental.daysToSec
import sk.ikim23.carrental.model.GraphModel
import sk.ikim23.carrental.model.ReplicationModel
import sk.ikim23.carrental.model.SummaryModel
import java.util.*

class SimManager(val rm: ReplicationModel, val sm: SummaryModel, val gm: GraphModel) : ISimListener {
    override val timeStep get() = rm.timeStep
    private val core = SimCore(this)
    private var nReps = 0
    private var curRep = 0
    private var configs = LinkedList<Pair<Int, Int>>()
    private var nBuses = 0
    private var nEmployees = 0

    override fun onDone(stats: Stats) {
        sm.take(stats)
        if (curRep < nReps) {
            core.start()
            curRep++
        } else {
            gm.take(nBuses, nEmployees, sm.lowSysTime(), sm.avgSysTime(), sm.uppSysTime())
            sm.clear()
            println("done $nBuses $nEmployees")
            stats.print()
            startNextConfig()
        }
    }

    override fun onStep(stats: Stats) {
        rm.onStep(stats, nBuses, nEmployees)
    }

    fun start(busFrom: Int, busTo: Int, emplFrom: Int, emplTo: Int, reps: Int) {
        if (configs.isNotEmpty() && curRep > 0) return
        for (b in busFrom..busTo) {
            for (e in emplFrom..emplTo) {
                configs.add(Pair(b, e))
            }
        }
        nReps = reps
        startNextConfig()
    }

    private fun startNextConfig() {
        curRep = 0
        val conf = configs.poll() ?: return
        nBuses = conf.first
        nEmployees = conf.second
        core.init(daysToSec(30), nBuses, nEmployees)
        start()
    }

    private fun start() {
        if (curRep > 0) return
        core.start()
        curRep++
    }

    fun unpause() {
        core.start()
    }

    fun pause() {
        core.pause()
    }

    fun stop() {
        nReps = 0
        curRep = 0
        configs.clear()
        core.stop()
    }
}