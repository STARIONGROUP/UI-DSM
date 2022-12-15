window.scrollToElement = (elementId, blockOption, inlineOption) => {
    const element = document.getElementById(elementId);

    if (element) {
        element.scrollIntoView({ block: blockOption, inline: inlineOption });
    }
}