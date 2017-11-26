#!/bin/bash
find -type f -name "*.bin" | while read file; do
	echo "${file}"
	rm ${file}
done