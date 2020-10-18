package sk.ikim23.carrental.core.impl

import sk.ikim23.carrental.core.Event

class SlowMoEvent(val core: SimCore, val listener: ISimListener) : Event(core, core.currentTime + listener.timeStep.value) {
    override fun exec() {
        if (listener.timeStep == ISimListener.Step.NONE) return
        listener.onStep(core.stats)
        core.sleep(100)
        if (core.hasEvents()) {
            core.addEvent(SlowMoEvent(core, listener))
        }
    }
}