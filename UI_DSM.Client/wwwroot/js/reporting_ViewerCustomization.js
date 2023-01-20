if(typeof String.prototype.replaceAll === "undefined") {
    String.prototype.replaceAll = function(match, replace) {
        return this.replace(new RegExp(match, 'g'), () => replace);
    }
}

window.ReportingViewerCustomization = {
    dotNetObjRef: null,
    setObjectRef: function(dotNetObjRef) {
        this.dotNetObjRef = dotNetObjRef;
    },
    //Change default Zoom level
    onBeforeRender: function(s, e) {
        //-1: Page Width
        //0: Whole Page
        //1: 100%
        e.reportPreview.zoom(1);
    },
    onPreviewClick: function(s, e) {
        if (e.Brick && e.GetBrickValue()) {
            var actions = e.GetBrickValue().split(",");
            actions.forEach(elem => 
                {
                    if (elem.startsWith("element=")) {
                        var id = elem.substring(8);
                        window.ReportingViewerCustomization.dotNetObjRef.invokeMethodAsync("SetSelectedElement", id);
                    }
                }
            );
        }
    }
}
