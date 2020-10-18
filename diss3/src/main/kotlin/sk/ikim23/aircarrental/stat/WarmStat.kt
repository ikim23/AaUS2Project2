package sk.ikim23.aircarrental.stat

import OSPABA.Simulation
import OSPStat.Stat
import sk.ikim23.aircarrental.sim.Config

class WarmStat(val stat: Stat, private val mySim: Simulation) : Stat() {
    override fun sampleSize(): Double {
        return stat.sampleSize()
    }

    override fun stdev(): Double {
        return stat.stdev()
    }

    override fun clear() {
        stat?.clear()
    }

    override fun toString(): String {
        return stat.toString()
    }

    override fun min(): Double {
        return stat.min()
    }

    override fun max(): Double {
        return stat.max()
    }

    override fun addSample(sample: Double) {
        if (mySim.currentTime() > Config.warmTime) {
            stat.addSample(sample)
        }
    }

    fun addSample(sample: Double, times: Int) {
        if (mySim.currentTime() > Config.warmTime) {
            for (i in 1..times) {
                stat.addSample(sample)
            }
        }
    }

    override fun mean(): Double {
        return stat.mean()
    }

    override fun addSamples(s: Stat) {
        stat.addSamples(s)
    }

    override fun confidenceInterval_90(): DoubleArray {
        if (stat.sampleSize() > 2.0) {
            return stat.confidenceInterval_90()
        }
        return doubleArrayOf(.0, .0)
    }

    override fun confidenceInterval_95(): DoubleArray {
        if (stat.sampleSize() > 2.0) {
            return stat.confidenceInterval_95()
        }
        return doubleArrayOf(.0, .0)
    }

    override fun confidenceInterval_99(): DoubleArray {
        if (stat.sampleSize() > 2.0) {
            return stat.confidenceInterval_99()
        }
        return doubleArrayOf(.0, .0)
    }
}