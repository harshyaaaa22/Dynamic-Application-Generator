// Layout Editor JavaScript

$(document).ready(function () {
    // Initialize CodeMirror editors
    var htmlEditor, cssEditor, jsEditor;

    if ($('#HtmlContent').length) {
        htmlEditor = CodeMirror.fromTextArea(document.getElementById('HtmlContent'), {
            lineNumbers: true,
            mode: 'htmlmixed',
            theme: 'monokai',
            indentWithTabs: false,
            indentUnit: 4,
            lineWrapping: true,
            extraKeys: { "Ctrl-Space": "autocomplete" }
        });

        // Make editor responsive
        htmlEditor.setSize("100%", 400);

        // Update textarea on editor change
        htmlEditor.on("change", function (instance) {
            instance.save();
        });
    }

    if ($('#CssContent').length) {
        cssEditor = CodeMirror.fromTextArea(document.getElementById('CssContent'), {
            lineNumbers: true,
            mode: 'css',
            theme: 'monokai',
            indentWithTabs: false,
            indentUnit: 4,
            lineWrapping: true,
            extraKeys: { "Ctrl-Space": "autocomplete" }
        });

        // Make editor responsive
        cssEditor.setSize("100%", 400);

        // Update textarea on editor change
        cssEditor.on("change", function (instance) {
            instance.save();
        });
    }

    if ($('#JsContent').length) {
        jsEditor = CodeMirror.fromTextArea(document.getElementById('JsContent'), {
            lineNumbers: true,
            mode: 'javascript',
            theme: 'monokai',
            indentWithTabs: false,
            indentUnit: 4,
            lineWrapping: true,
            extraKeys: { "Ctrl-Space": "autocomplete" }
        });

        // Make editor responsive
        jsEditor.setSize("100%", 400);

        // Update textarea on editor change
        jsEditor.on("change", function (instance) {
            instance.save();
        });
    }

    // Tab switching functionality
    $('#editorTabs a').on('click', function (e) {
        e.preventDefault();
        $(this).tab('show');

        // Refresh the CodeMirror instance when switching to its tab
        // (This helps fix any display issues)
        if ($(this).attr('href') === '#html' && htmlEditor) {
            setTimeout(function () { htmlEditor.refresh(); }, 10);
        } else if ($(this).attr('href') === '#css' && cssEditor) {
            setTimeout(function () { cssEditor.refresh(); }, 10);
        } else if ($(this).attr('href') === '#js' && jsEditor) {
            setTimeout(function () { jsEditor.refresh(); }, 10);
        }
    });

    // Form submission validation
    $('form').on('submit', function (e) {
        // Make sure all CodeMirror instances save their content
        if (htmlEditor) htmlEditor.save();
        if (cssEditor) cssEditor.save();
        if (jsEditor) jsEditor.save();
    });

    // Initialize tooltips
    $('[data-toggle="tooltip"]').tooltip();
});