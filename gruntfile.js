module.exports = function (grunt) {
    grunt.initConfig({
        markdown: {
            all: {
                files: [
                    {
                        expand: true,
                        src: ['**/*.md', '!node_modules/**', '!html/**' ],
                        dest: 'html/'
                    }
                ],
                options: {
                    markdownOptions: {
                        highlight: 'manual'
                    }
                }
            }
        }
    });

    grunt.loadNpmTasks('grunt-markdown');

    grunt.registerTask('default', ['markdown:all']);
};