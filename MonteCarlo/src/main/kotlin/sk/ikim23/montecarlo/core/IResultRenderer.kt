package sk.ikim23.montecarlo.core

interface IResultRenderer<T> {
    fun render(results: List<T>)
}