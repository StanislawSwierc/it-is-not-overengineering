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
                    plugins: {
                        'markdown-it-anchor': {
                            level: 1
                        }
                    }
                }
            }
        },
        inline: {
            all: {
                src: ['**/*.html', '!node_modules/**', '!html_inline/**'],
                dest: 'html_inline/'
            }
        },
        inlinecss: {
            all: {
                options: {
                },
                files: [
                    {
                        expand: true,
                        src: ['**/*.html', '!node_modules/**', '!html/**'],
                        dest: 'html_inline/'
                    }
                ],
            }
        }
    });

    grunt.loadNpmTasks('grunt-markdown-it');
    grunt.loadNpmTasks('grunt-inline-css');
    grunt.loadNpmTasks('grunt-inline');

    grunt.registerTask('html_inline', ['markdownit:all', 'inline:all']);
    grunt.registerTask('html', ['markdownit:all']);
};