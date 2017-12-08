const url = require('url');
const path = require('path');

module.exports = function (grunt) {
    grunt.initConfig({
        markdownit: {
            all: {
                files: [
                    {
                        expand: true,
                        src: [
                            '2013/**/*.md',
                            '2014/**/*.md',
                            '2017/**/*.md'
                        ],
                        rename: function (dest, src) {
                            return src.replace(/\.md$/, ".html");
                        }
                    }
                ],
                options: {
                    highlightjs: true,
                    html: true,
                    linkify: true,
                    replaceLink: function (link, env) {
                        var linkObj = url.parse(link);
                        if (linkObj.protocol) {
                            return link;
                        } else {
                            linkObj = url.parse(
                                "http://stanislawswierc.github.io/it-is-not-overengineering/"
                                + path.dirname(env.file.dest)
                                + '/'
                                + link.replace(/^\.?\//, ''))
                            return linkObj.href;
                        }
                    },
                    plugins: {
                        'markdown-it-anchor': {
                            level: 1
                        },
                        'markdown-it-replace-link': {},
                        'markdown-it-attrs': {},
                        'markdown-it-linkify-images': {}
                    }
                }
            }
        }
    });

    grunt.loadNpmTasks('grunt-markdown-it');

    grunt.registerTask('html', ['markdownit:all']);
};