angular.module("umbraco").controller("EventCalendar.EventEditController",
        function ($scope, $routeParams, eventResource, locationResource, notificationsService, assetsService, tinyMceService, $timeout) {

            var validElements = "";
            var extendedValidElements = "";
            var invalidElements = "";
            var plugins = "";
            var editorConfig = {
                toolbar: ["code", "undo", "redo", "cut", "styleselect", "bold", "italic", "alignleft", "aligncenter", "alignright", "bullist", "numlist", "link", "umbmediapicker", "umbmacro", "table", "umbembeddialog"],
                stylesheets: [],
                dimensions: { height: 400, width: 250 }
            };
            var toolbar = "";
            var stylesheets = [];
            var styleFormats = [];

            //stores a reference to the editor
            var tinyMceEditor = [];

            tinyMceService.configuration().then(function(tinyMceConfig){
                //config value from general tinymce.config file
                validElements = tinyMceConfig.validElements;

                //These are absolutely required in order for the macros to render inline
                //we put these as extended elements because they get merged on top of the normal allowed elements by tiny mce
                extendedValidElements = "@[id|class|style],-div[id|dir|class|align|style],ins[datetime|cite],-ul[class|style],-li[class|style]";

                invalidElements = tinyMceConfig.inValidElements;
                plugins = _.map(tinyMceConfig.plugins, function(plugin){ 
                    if(plugin.useOnFrontend){
                        return plugin.name;   
                    }
                }).join(" ");                

                if(!editorConfig || angular.isString(editorConfig)){
                    editorConfig = tinyMceService.defaultPrevalues();
                }

                //config value on the data type
                toolbar = editorConfig.toolbar.join(" | ");                
            });

            //get a calendar id -> service
            eventResource.getById($routeParams.id.replace("e-","")).then(function (response) {
                $scope.event = response.data;

                //Create the tabs for every language etc
                $scope.tabs = [{ id: "Content", label: "Content" }];
                angular.forEach($scope.event.descriptions, function (value, key) {
                    this.push({ id: key, label: value.culture });
                }, $scope.tabs);

                //queue file loading
                assetsService.loadJs("lib/tinymce/tinymce.min.js").then(function () {
                    angular.forEach($scope.event.descriptions, function (description) {
                        loadTinyMce(description.culture);
                    });
                });

                //Load js library add set the date values for starttime/endtime
                assetsService
                    .loadJs("/App_Plugins/EventCalendar/scripts/moment.min.js")
                    .then(function () {
                        $('#datetimepicker1 input').val(moment($scope.event.starttime).format('MM/DD/YYYY HH:mm:ss'));
                        $('#datetimepicker2 input').val(moment($scope.event.endtime).format('MM/DD/YYYY HH:mm:ss'));
                    });
            }, function (response) {
                notificationsService.error("Error", $scope.currentNode.name + " could not be loaded");
            });

            //Load all locations
            locationResource.getall().then(function (response) {
                $scope.locations = response.data;
            }, function (response) {
                notificationsService.error("Error", "Could not load locations");
            });

            assetsService.loadCss("/App_Plugins/EventCalendar/css/bootstrap-datetimepicker.min.css");
            assetsService.loadCss("/App_Plugins/EventCalendar/css/bootstrap-switch.css");
            assetsService.loadCss("/App_Plugins/EventCalendar/css/eventcalendar.custom.css");            

            assetsService
                .loadJs("/App_Plugins/EventCalendar/scripts/bootstrap-datetimepicker.min.js")
                .then(function () {
                    //this function will execute when all dependencies have loaded
                    $('#datetimepicker1').datetimepicker({
                        language: 'en'
                    });

                    $('#datetimepicker2').datetimepicker({
                        language: 'en'
                    });

                    $('#datetimepicker1').on('changeDate', function (e) {
                        var d = moment(e.date).format('MM/DD/YYYY HH:mm:ss');
                        $scope.event.starttime = d;
                    });
                    $('#datetimepicker2').on('changeDate', function (e) {
                        var d = moment(e.date).format('MM/DD/YYYY HH:mm:ss');
                        $scope.event.endtime = d;
                    });
                });

            assetsService
                .loadJs("/App_Plugins/EventCalendar/scripts/bootstrap-switch.min.js")
                .then(function () {
                    if($scope.event) {
                        $('#allday').bootstrapSwitch('setState', $scope.event.allday, true);
                    }
                    $('#allday').on('switch-change', function (e, data) {
                        $scope.event.allday = data.value;
                    });
                });

            $scope.save = function (event) {
                eventResource.save(event).then(function (response) {
                    $scope.event = response.data;

                    notificationsService.success("Success", event.title + " has been saved");
                }, function (response) {
                    notificationsService.error("Error", event.title + " could not be saved");
                });
            };

            /** Loads in the editor */
            function loadTinyMce(alias) {

                //we need to add a timeout here, to force a redraw so TinyMCE can find
                //the elements needed
                $timeout(function () {
                    tinymce.DOM.events.domLoaded = true;
                    tinymce.init({
                        mode: "exact",
                        elements: alias + "_rte",
                        skin: "umbraco",
                        plugins: plugins,
                        valid_elements: validElements,
                        invalid_elements: invalidElements,
                        extended_valid_elements: extendedValidElements,
                        menubar: false,
                        statusbar: false,
                        height: editorConfig.dimensions.height,
                        width: editorConfig.dimensions.width,
                        toolbar: toolbar,
                        content_css: stylesheets.join(','),
                        relative_urls: false,
                        style_formats: styleFormats,
                        setup: function (editor) {

                            //set the reference
                            tinyMceEditor.push(editor);

                            //We need to listen on multiple things here because of the nature of tinymce, it doesn't 
                            //fire events when you think!
                            //The change event doesn't fire when content changes, only when cursor points are changed and undo points
                            //are created. the blur event doesn't fire if you insert content into the editor with a button and then 
                            //press save. 
                            //We have a couple of options, one is to do a set timeout and check for isDirty on the editor, or we can 
                            //listen to both change and blur and also on our own 'saving' event. I think this will be best because a 
                            //timer might end up using unwanted cpu and we'd still have to listen to our saving event in case they clicked
                            //save before the timeout elapsed.
                            editor.on('change', function (e) {
                                angularHelper.safeApply($scope, function () {
                                    $scope.model.value = editor.getContent();
                                });
                            });
                            editor.on('blur', function (e) {
                                angularHelper.safeApply($scope, function () {
                                    $scope.model.value = editor.getContent();
                                });
                            });

                            //Create the insert media plugin
                            tinyMceService.createMediaPicker(editor, $scope);

                            //Create the embedded plugin
                            tinyMceService.createInsertEmbeddedMedia(editor, $scope);

                            //Create the insert link plugin
                            tinyMceService.createLinkPicker(editor, $scope);

                            //Create the insert macro plugin
                            tinyMceService.createInsertMacro(editor, $scope);
                        }
                    });
                }, 200);
            }

            //listen for formSubmitting event (the result is callback used to remove the event subscription)
            var unsubscribe = $scope.$on("formSubmitting", function () {

                //TODO: Here we should parse out the macro rendered content so we can save on a lot of bytes in data xfer
                // we do parse it out on the server side but would be nice to do that on the client side before as well.
                $scope.model.value = tinyMceEditor.getContent();
            });

            //when the element is disposed we need to unsubscribe!
            // NOTE: this is very important otherwise if this is part of a modal, the listener still exists because the dom 
            // element might still be there even after the modal has been hidden.
            $scope.$on('$destroy', function () {
                unsubscribe();
            });

        });