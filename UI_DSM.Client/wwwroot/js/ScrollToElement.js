window.scrollToElement = (elementId, blockOption, inlineOption) => {
    const element = document.getElementById(elementId);

    if (element) {
        const highlightClass = 'highlight-element';

        element.scrollIntoView({ block: blockOption, inline: inlineOption });
        element.classList.add(highlightClass);

        setTimeout(() => {
            element.classList.remove(highlightClass);
        }, 3000);
    }
}