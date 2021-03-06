COMPILE_TARGET = "debug"
require "BuildUtils.rb"
require 'albacore'

BUILD_NUMBER_BASE = "0.9.0"
PRODUCT = "StoryTeller"
COPYRIGHT = 'Released under the Apache 2.0 License';
COMMON_ASSEMBLY_INFO = 'source/CommonAssemblyInfo.cs';

versionNumber = ENV["CCNetLabel"].nil? ? 0 : ENV["CCNetLabel"]



task :default => [:compile, :unit_test]

desc "Update the version information for the build"
assemblyinfo :version do |asm|
  asm_version = BUILD_NUMBER_BASE + ".0"
  
  begin
	gittag = `git describe --long --tags`.chomp 	# looks something like v0.1.0-63-g92228f4
    gitnumberpart = /-(\d+)-/.match(gittag)
    gitnumber = gitnumberpart.nil? ? '0' : gitnumberpart[1]
    commit = (ENV["BUILD_VCS_NUMBER"].nil? ? `git log -1 --pretty=format:%H` : ENV["BUILD_VCS_NUMBER"])
  rescue
    commit = "git unavailable"
    gitnumber = "0"
  end
  build_number = "#{BUILD_NUMBER_BASE}.#{gitnumber}"
  tc_build_number = ENV["BUILD_NUMBER"]
  puts "##teamcity[buildNumber '#{build_number}-#{tc_build_number}']" unless tc_build_number.nil?
  asm.trademark = commit
  asm.product_name = "#{PRODUCT} #{gittag}"
  asm.description = build_number
  asm.version = asm_version
  asm.file_version = build_number
  asm.custom_attributes :AssemblyInformationalVersion => asm_version
  asm.copyright = COPYRIGHT
  asm.output_file = COMMON_ASSEMBLY_INFO
end

task :compile => :version do
  MSBuildRunner.compile :compilemode => COMPILE_TARGET, :solutionfile => 'source/StoryTeller.sln'
end

task :unit_test => :compile do
  runner = NUnitRunner.new :compilemode => COMPILE_TARGET
  runner.executeTests ['StoryTeller.Testing']
end

task :package  do
  require 'fileutils'
  FileUtils.rm_rf 'deploy'

  Dir.mkdir 'deploy'

  FileUtils.cp 'source\StoryTellerUI\bin\Debug\ICSharpCode.AvalonEdit.dll', 'deploy'
  FileUtils.cp 'source\StoryTellerUI\bin\Debug\ICSharpCode.Core.dll', 'deploy'
  FileUtils.cp 'source\StoryTellerUI\bin\Debug\ICSharpCode.Core.Presentation.dll', 'deploy'
  FileUtils.cp 'source\StoryTellerUI\bin\Debug\ICSharpCode.Core.WinForms.dll', 'deploy'
  FileUtils.cp 'source\StoryTellerUI\bin\Debug\ICSharpCode.SharpDevelop.dll', 'deploy'
  FileUtils.cp 'source\StoryTellerUI\bin\Debug\ICSharpCode.TextEditor.dll', 'deploy'
  FileUtils.cp 'source\StoryTellerUI\bin\Debug\StoryTeller.UserInterface.dll', 'deploy'
  FileUtils.cp 'source\StoryTellerUI\bin\Debug\StoryTeller.UserInterface.pdb', 'deploy'
  FileUtils.cp 'source\StoryTellerUI\bin\Debug\XmlEditor.dll', 'deploy'
  FileUtils.cp 'source\StoryTellerUI\bin\Debug\StoryTeller.dll', 'deploy'
  FileUtils.cp 'source\StoryTellerUI\bin\Debug\StoryTeller.pdb', 'deploy'
  FileUtils.cp 'source\StoryTellerUI\bin\Debug\StoryTellerUI.exe', 'deploy'
  FileUtils.cp 'source\StoryTellerUI\bin\Debug\StructureMap.dll', 'deploy'
end



