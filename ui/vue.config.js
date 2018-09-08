module.exports = {
    chainWebpack: config => {
        config.module
        .rule("fonts")
        .test(/\.(woff(2)?|ttf|eot|svg)(\?v=\d+\.\d+\.\d+)?$/)
        .use("fileloader")
        .loader("file-loader")
        .options({
            name: "[name].[ext]",
            outputPath: "fonts/"
        });
    }
}