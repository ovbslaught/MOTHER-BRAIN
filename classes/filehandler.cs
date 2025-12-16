using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Reflection;
using runnerDotNet;
namespace runnerDotNet
{
	public partial class RunnerFileHandler : XClass
	{
		public ProjectSettings pSet;
		public dynamic field;
		protected dynamic _datasource;
		protected dynamic _fs;
		protected dynamic options = XVar.Array();
		public dynamic formStamp;
		public RunnerFileHandler(dynamic _param_field, dynamic _param_pSet_packed, dynamic _param_formStamp = null)
		{
			#region packeted values
			ProjectSettings _param_pSet = XVar.UnPackProjectSettings(_param_pSet_packed);
			#endregion

			#region default values
			if(_param_formStamp as Object == null) _param_formStamp = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			ProjectSettings pSet = XVar.Clone(_param_pSet);
			dynamic formStamp = XVar.Clone(_param_formStamp);
			#endregion

			this.pSet = XVar.UnPackProjectSettings(pSet);
			this.field = XVar.Clone(field);
			this.formStamp = XVar.Clone(formStamp);
		}
		protected virtual XVar acceptUploadedFile(dynamic _param_uploadedFile)
		{
			#region pass-by-value parameters
			dynamic uploadedFile = XVar.Clone(_param_uploadedFile);
			#endregion

			dynamic errorMessage = null, file = XVar.Array(), fileId = null, fs = null, resizedFileName = null, thumbnailFileName = null;
			file = XVar.Clone(XVar.Array());
			errorMessage = new XVar("");
			if(XVar.Pack(uploadedFile["error"]))
			{
				file.InitAndSetArrayItem(this.codeToMessage((XVar)(uploadedFile["error"])), "error");
				return file;
			}
			if(XVar.Pack(!(XVar)(this.validateFile((XVar)(uploadedFile["name"]), (XVar)(uploadedFile["size"]), ref errorMessage))))
			{
				file.InitAndSetArrayItem(errorMessage, "error");
				return file;
			}
			file.InitAndSetArrayItem(RunnerFileHandler.fileType((XVar)(uploadedFile)), "type");
			file.InitAndSetArrayItem(uploadedFile["size"], "size");
			file.InitAndSetArrayItem(this.trimFilename((XVar)(uploadedFile["name"])), "usrName");
			if(XVar.Pack(CommonFunctions.isImageType((XVar)(file["type"]))))
			{
				dynamic img_height = null, img_size = XVar.Array(), img_width = null;
				img_size = XVar.Clone(this.getImageDimensions((XVar)(uploadedFile)));
				img_width = XVar.Clone(img_size[0]);
				img_height = XVar.Clone(img_size[1]);
				if((XVar)((XVar)((XVar)(!(XVar)(GlobalVars.resizeImagesOnClient))  && (XVar)(img_width))  && (XVar)(img_height))  && (XVar)(this.pSet.getResizeOnUpload((XVar)(this.field))))
				{
					resizedFileName = XVar.Clone(this.resizeUploadedImage((XVar)(uploadedFile), (XVar)(img_width), (XVar)(img_height), (XVar)(this.pSet.getNewImageSize((XVar)(this.field)))));
					if(XVar.Pack(resizedFileName))
					{
						file.InitAndSetArrayItem(MVCFunctions.filesize((XVar)(resizedFileName)), "size");
					}
				}
				if((XVar)((XVar)(img_width)  && (XVar)(img_height))  && (XVar)(this.pSet.getCreateThumbnail((XVar)(this.field))))
				{
					thumbnailFileName = XVar.Clone(this.resizeUploadedImage((XVar)(uploadedFile), (XVar)(img_width), (XVar)(img_height), (XVar)(this.pSet.getThumbnailSize((XVar)(this.field)))));
					if(XVar.Pack(thumbnailFileName))
					{
						file.InitAndSetArrayItem(MVCFunctions.filesize((XVar)(thumbnailFileName)), "thumbnail_size");
						file.InitAndSetArrayItem(file["type"], "thumbnail_type");
					}
				}
			}
			fs = XVar.Clone(this.getFilesystem());
			if(XVar.Pack(!(XVar)(resizedFileName)))
			{
				fileId = XVar.Clone(fs.saveUploadedFile((XVar)(uploadedFile), (XVar)(file["usrName"])));
			}
			else
			{
				fileId = XVar.Clone(fs.moveFile((XVar)(resizedFileName), (XVar)(file["usrName"])));
				MVCFunctions.unlink((XVar)(resizedFileName));
			}
			if(XVar.Pack(!(XVar)(fileId)))
			{
				file.InitAndSetArrayItem(fs.lastError(), "error");
			}
			else
			{
				file.InitAndSetArrayItem(fileId, "name");
			}
			if(XVar.Pack(thumbnailFileName))
			{
				fileId = XVar.Clone(fs.moveFile((XVar)(thumbnailFileName), (XVar)(MVCFunctions.Concat("th", file["usrName"]))));
				MVCFunctions.unlink((XVar)(thumbnailFileName));
				if(XVar.Pack(fileId))
				{
					file.InitAndSetArrayItem(fileId, "thumbnail");
				}
			}
			return file;
		}
		public virtual XVar resetTo(dynamic _param_defaultValue)
		{
			#region pass-by-value parameters
			dynamic defaultValue = XVar.Clone(_param_defaultValue);
			#endregion

			dynamic filesToDelete = XVar.Array(), formData = XVar.Array(), names = XVar.Array(), userFilesArray = XVar.Array();
			userFilesArray = XVar.Clone(MVCFunctions.runner_json_decode((XVar)(defaultValue)));
			names = XVar.Clone(XVar.Array());
			if(XVar.Pack(MVCFunctions.is_array((XVar)(userFilesArray))))
			{
				foreach (KeyValuePair<XVar, dynamic> userFile in userFilesArray.GetEnumerator())
				{
					dynamic fileData = XVar.Array();
					if(XVar.Pack(!(XVar)(userFile.Value.KeyExists("name"))))
					{
						continue;
					}
					fileData = XVar.Clone(this.getFormData((XVar)(userFile.Value["name"])));
					if(XVar.Pack(fileData))
					{
						names.InitAndSetArrayItem(userFile.Value["name"], null);
						fileData.InitAndSetArrayItem(false, "deleted");
						this.setFormData((XVar)(userFile.Value["name"]), (XVar)(fileData));
					}
				}
			}
			formData = XVar.Clone(XSession.Session[MVCFunctions.Concat("mupload_", this.formStamp)]);
			if(XVar.Pack(!(XVar)(formData)))
			{
				return true;
			}
			filesToDelete = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> fileArray in formData.GetEnumerator())
			{
				if(XVar.Pack(!(XVar)(MVCFunctions.in_array((XVar)(fileArray.Key), (XVar)(names)))))
				{
					filesToDelete.InitAndSetArrayItem(fileArray.Key, null);
				}
			}
			foreach (KeyValuePair<XVar, dynamic> fileName in filesToDelete.GetEnumerator())
			{
				this.delete((XVar)(fileName.Value));
			}
			return true;
		}
		public virtual XVar delete(dynamic _param_filename)
		{
			#region pass-by-value parameters
			dynamic filename = XVar.Clone(_param_filename);
			#endregion

			dynamic fileData = XVar.Array();
			fileData = XVar.Clone(this.getFormData((XVar)(filename)));
			if(XVar.Pack(!(XVar)(fileData)))
			{
				return false;
			}
			if(XVar.Pack(!(XVar)(fileData["fromDB"])))
			{
				dynamic sessionFile = XVar.Array(), storageProvider = null;
				sessionFile = XVar.Clone(fileData["file"]);
				storageProvider = XVar.Clone(this.getFilesystem());
				storageProvider.delete((XVar)(sessionFile["name"]));
				if(XVar.Pack(sessionFile["thumbnail"]))
				{
					storageProvider.delete((XVar)(sessionFile["thumbnail"]));
				}
				this.deleteFormData((XVar)(filename));
			}
			else
			{
				fileData.InitAndSetArrayItem(true, "deleted");
				this.setFormData((XVar)(filename), (XVar)(fileData));
			}
			return true;
		}
		public virtual XVar acceptUpload(dynamic _param_uploadedFiles, dynamic _param_wrapWithFiles = null)
		{
			#region default values
			if(_param_wrapWithFiles as Object == null) _param_wrapWithFiles = new XVar(false);
			#endregion

			#region pass-by-value parameters
			dynamic uploadedFiles = XVar.Clone(_param_uploadedFiles);
			dynamic wrapWithFiles = XVar.Clone(_param_wrapWithFiles);
			#endregion

			dynamic result = XVar.Array();
			result = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> uploadedFile in uploadedFiles.GetEnumerator())
			{
				dynamic file = XVar.Array();
				file = XVar.Clone(this.acceptUploadedFile((XVar)(uploadedFile.Value)));
				if(XVar.Pack(!(XVar)(file.KeyExists("error"))))
				{
					dynamic fileData = XVar.Array();
					fileData = XVar.Clone(XVar.Array());
					fileData.InitAndSetArrayItem(file, "file");
					fileData.InitAndSetArrayItem(false, "deleted");
					fileData.InitAndSetArrayItem(false, "fromDB");
					this.setFormData((XVar)(file["usrName"]), (XVar)(fileData));
				}
				result.InitAndSetArrayItem(this.returnFileDescriptor((XVar)(file)), null);
			}
			if(XVar.Pack(wrapWithFiles))
			{
				result = XVar.Clone(new XVar("files", result));
			}
			MVCFunctions.Header("Vary", "Accept");
			MVCFunctions.Header("Content-type", "application/json");
			MVCFunctions.Echo(MVCFunctions.runner_json_encode((XVar)(result)));

			return null;
		}
		protected virtual XVar returnFileDescriptor(dynamic _param_file)
		{
			#region pass-by-value parameters
			dynamic file = XVar.Clone(_param_file);
			#endregion

			dynamic fs = null, lastModified = null, returnFile = XVar.Array(), urlParams = XVar.Array();
			returnFile = XVar.Clone(XVar.Array());
			if(XVar.Pack(file.KeyExists("error")))
			{
				returnFile.InitAndSetArrayItem(file["error"], "error");
				return returnFile;
			}
			returnFile.InitAndSetArrayItem(true, "isImg");
			returnFile.InitAndSetArrayItem(file["usrName"], "name");
			returnFile.InitAndSetArrayItem(file["type"], "type");
			returnFile.InitAndSetArrayItem(file["size"], "size");
			urlParams = XVar.Clone(XVar.Array());
			urlParams.InitAndSetArrayItem(file["usrName"], "file");
			urlParams.InitAndSetArrayItem(this.table(), "table");
			urlParams.InitAndSetArrayItem(this.field, "field");
			urlParams.InitAndSetArrayItem(this.formStamp, "fkey");
			fs = XVar.Clone(this.getFilesystem());
			lastModified = XVar.Clone(MVCFunctions.time());
			if(XVar.Pack(fs.fast()))
			{
				dynamic fsInfo = XVar.Array();
				fsInfo = XVar.Clone(fs.getFileInfo((XVar)(file["name"])));
				if((XVar)(fsInfo)  && (XVar)(fsInfo["lastModified"]))
				{
					lastModified = XVar.Clone(fsInfo["lastModified"]);
				}
			}
			urlParams.InitAndSetArrayItem(CommonFunctions.fileAttrHash((XVar)(this.formStamp), (XVar)(file["size"]), (XVar)(lastModified)), "hash");
			returnFile.InitAndSetArrayItem(MVCFunctions.GetTableLink(new XVar("file"), new XVar(""), (XVar)(CommonFunctions.prepareUrlQuery((XVar)(urlParams)))), "url");
			if(XVar.Pack(!(XVar)(CommonFunctions.CheckImageExtension((XVar)(file["usrName"])))))
			{
				urlParams.InitAndSetArrayItem(1, "icon");
			}
			else
			{
				urlParams.InitAndSetArrayItem(1, "thumbnail");
			}
			returnFile.InitAndSetArrayItem(MVCFunctions.GetTableLink(new XVar("file"), new XVar(""), (XVar)(CommonFunctions.prepareUrlQuery((XVar)(urlParams)))), "thumbnail_url");
			return returnFile;
		}
		protected virtual XVar validateFile(dynamic _param_filename, dynamic _param_fileSize, ref dynamic errorMessage)
		{
			#region pass-by-value parameters
			dynamic filename = XVar.Clone(_param_filename);
			dynamic fileSize = XVar.Clone(_param_fileSize);
			#endregion

			dynamic acceptedTypes = null, maxFileSize = null, maxNumberOfFiles = null, maxTotalFileSize = null;
			if(XVar.Pack(!(XVar)(filename)))
			{
				errorMessage = XVar.Clone(CommonFunctions.mlang_message(new XVar("ERROR_MISSING_FILE_NAME")));
				return false;
			}
			acceptedTypes = XVar.Clone(this.pSet.getAcceptFileTypes((XVar)(this.field)));
			if(XVar.Pack(acceptedTypes))
			{
				dynamic ext = null;
				ext = XVar.Clone(MVCFunctions.strtoupper((XVar)(CommonFunctions.getFileExtension((XVar)(filename)))));
				if((XVar)(XVar.Equals(XVar.Pack(MVCFunctions.array_search((XVar)(ext), (XVar)(acceptedTypes))), XVar.Pack(false)))  && (XVar)(XVar.Equals(XVar.Pack(MVCFunctions.array_search((XVar)(MVCFunctions.Concat(".", ext)), (XVar)(acceptedTypes))), XVar.Pack(false))))
				{
					errorMessage = XVar.Clone(CommonFunctions.mlang_message(new XVar("ERROR_ACCEPT_FILE_TYPES")));
					return false;
				}
			}
			maxFileSize = XVar.Clone(this.pSet.getMaxFileSize((XVar)(this.field)));
			if((XVar)(maxFileSize)  && (XVar)(maxFileSize * 1024 < fileSize))
			{
				errorMessage = XVar.Clone(MVCFunctions.mysprintf((XVar)(CommonFunctions.mlang_message(new XVar("ERROR_MAX_FILE_SIZE"))), (XVar)(new XVar(0, maxFileSize))));
				return false;
			}
			maxTotalFileSize = XVar.Clone(this.pSet.getMaxTotalFilesSize((XVar)(this.field)));
			if(XVar.Pack(maxTotalFileSize))
			{
				if(maxTotalFileSize * 1024 < this.getUploadFilesSize() + fileSize)
				{
					errorMessage = XVar.Clone(MVCFunctions.mysprintf((XVar)(CommonFunctions.mlang_message(new XVar("ERROR_MAX_TOTAL_FILE_SIZE"))), (XVar)(new XVar(0, maxTotalFileSize))));
					return false;
				}
			}
			maxNumberOfFiles = XVar.Clone(this.pSet.getMaxNumberOfFiles((XVar)(this.field)));
			if((XVar)(maxNumberOfFiles)  && (XVar)(maxNumberOfFiles <= this.getUploadFilesCount()))
			{
				errorMessage = XVar.Clone(MVCFunctions.mysprintf((XVar)(CommonFunctions.mlang_message(new XVar("ERROR_MAX_NUMBER_OF_FILES_MANY"))), (XVar)(new XVar(0, maxNumberOfFiles))));
				return false;
			}
			return true;
		}
		protected virtual XVar getUploadFilesSize()
		{
			dynamic result = null, sessKey = null;
			sessKey = XVar.Clone(MVCFunctions.Concat("mupload_", this.formStamp));
			if(XVar.Pack(!(XVar)(XSession.Session[sessKey])))
			{
				return 0;
			}
			result = new XVar(0);
			foreach (KeyValuePair<XVar, dynamic> fileArray in XSession.Session[sessKey].GetEnumerator())
			{
				if(XVar.Pack(!(XVar)(fileArray.Value["deleted"])))
				{
					result += fileArray.Value["file"]["size"];
				}
			}
			return result;
		}
		protected virtual XVar getUploadFilesCount()
		{
			dynamic result = null, sessKey = null;
			sessKey = XVar.Clone(MVCFunctions.Concat("mupload_", this.formStamp));
			if(XVar.Pack(!(XVar)(XSession.Session[sessKey])))
			{
				return 0;
			}
			result = new XVar(0);
			foreach (KeyValuePair<XVar, dynamic> fileArray in XSession.Session[sessKey].GetEnumerator())
			{
				if(XVar.Pack(!(XVar)(fileArray.Value["deleted"])))
				{
					++(result);
				}
			}
			return result;
		}
		protected virtual XVar getFormData(dynamic _param_filename)
		{
			#region pass-by-value parameters
			dynamic filename = XVar.Clone(_param_filename);
			#endregion

			dynamic formInfo = XVar.Array();
			formInfo = XSession.Session[MVCFunctions.Concat("mupload_", this.formStamp)];
			if(XVar.Pack(!(XVar)(formInfo)))
			{
				return XVar.Array();
			}
			return formInfo[filename];
		}
		protected virtual XVar setFormData(dynamic _param_filename, dynamic _param_data)
		{
			#region pass-by-value parameters
			dynamic filename = XVar.Clone(_param_filename);
			dynamic data = XVar.Clone(_param_data);
			#endregion

			dynamic formInfo = XVar.Array(), sessKey = null;
			sessKey = XVar.Clone(MVCFunctions.Concat("mupload_", this.formStamp));
			if(XVar.Pack(!(XVar)(XSession.Session[sessKey])))
			{
				XSession.Session[sessKey] = XVar.Array();
			}
			formInfo = XSession.Session[sessKey];
			if(XVar.Pack(!(XVar)(data)))
			{
				formInfo.Remove(filename);
			}
			else
			{
				formInfo.InitAndSetArrayItem(data, filename);
			}

			return null;
		}
		protected virtual XVar deleteFormData(dynamic _param_filename)
		{
			#region pass-by-value parameters
			dynamic filename = XVar.Clone(_param_filename);
			#endregion

			this.setFormData((XVar)(filename), new XVar(null));

			return null;
		}
		protected virtual XVar getTableRecord(dynamic _param_keys)
		{
			#region pass-by-value parameters
			dynamic keys = XVar.Clone(_param_keys);
			#endregion

			dynamic cipherer = null, dataSource = null, dc = null, qResult = null;
			dc = XVar.Clone(new DsCommand());
			if((XVar)(Security.fieldIsUserpic((XVar)(this.table()), (XVar)(this.field)))  && (XVar)(!(XVar)(this.tableRightsAvailable())))
			{
				dc.filter = XVar.Clone(Security.currentUserCondition());
			}
			else
			{
				dc.filter = XVar.Clone(this.securityCondition());
				dc.keys = XVar.Clone(keys);
			}
			dataSource = XVar.Clone(this.getDataSource());
			qResult = XVar.Clone(dataSource.getSingle((XVar)(dc)));
			if(XVar.Pack(!(XVar)(qResult)))
			{
				return null;
			}
			cipherer = XVar.Clone(new RunnerCipherer((XVar)(this.table()), (XVar)(this.pSet)));
			return cipherer.DecryptFetchedArray((XVar)(qResult.fetchAssoc()));
		}
		protected virtual XVar getFileInfo(dynamic _param_filename, dynamic _param_thumbnail, dynamic _param_keys)
		{
			#region pass-by-value parameters
			dynamic filename = XVar.Clone(_param_filename);
			dynamic thumbnail = XVar.Clone(_param_thumbnail);
			dynamic keys = XVar.Clone(_param_keys);
			#endregion

			dynamic data = XVar.Array(), filesArray = XVar.Array(), fs = null;
			if(XVar.Pack(this.formStamp))
			{
				data = XVar.Clone(this.getFormData((XVar)(filename)));
				if(XVar.Pack(data))
				{
					return data["file"];
				}
				else
				{
					return null;
				}
			}
			if(XVar.Pack(!(XVar)(keys)))
			{
				return null;
			}
			data = this.getTableRecord((XVar)(keys));
			if(XVar.Pack(this.databaseFile()))
			{
				dynamic connection = null, ds = null, fileData = XVar.Array(), filenameField = null;
				fileData = XVar.Clone(XVar.Array());
				ds = XVar.Clone(this.getDataSource());
				connection = XVar.Clone(ds.getConnection());
				if(XVar.Pack(thumbnail))
				{
					dynamic thumbField = null;
					thumbField = XVar.Clone(this.pSet.getThumbnailField((XVar)(this.field)));
					if((XVar)(thumbField)  && (XVar)(data[thumbField]))
					{
						fileData.InitAndSetArrayItem(connection.stripSlashesBinary((XVar)(data[thumbField])), "value");
					}
				}
				if(XVar.Pack(!(XVar)(fileData["value"])))
				{
					fileData.InitAndSetArrayItem(connection.stripSlashesBinary((XVar)(data[this.field])), "value");
				}
				if(XVar.Pack(!(XVar)(fileData["value"])))
				{
					return null;
				}
				filenameField = XVar.Clone(this.pSet.getFilenameField((XVar)(this.field)));
				if((XVar)(filenameField)  && (XVar)(data[filenameField]))
				{
					fileData.InitAndSetArrayItem(data[filenameField], "usrName");
				}
				else
				{
					fileData.InitAndSetArrayItem("file.bin", "usrName");
				}
				fileData.InitAndSetArrayItem(MVCFunctions.strlen_bin((XVar)(fileData["value"])), "size");
				fileData.InitAndSetArrayItem(MVCFunctions.SupposeImageType((XVar)(fileData["value"])), "type");
				return fileData;
			}
			fs = XVar.Clone(this.getFilesystem());
			filesArray = XVar.Clone(MVCFunctions.runner_json_decode((XVar)(data[this.field])));
			if((XVar)(!(XVar)(MVCFunctions.is_array((XVar)(filesArray))))  || (XVar)(MVCFunctions.count(filesArray) == 0))
			{
				dynamic dbFilename = null, fileDesc = XVar.Array(), filePath = null, fsFile = XVar.Array();
				filesArray = XVar.Clone(XVar.Array());
				dbFilename = XVar.Clone(data[this.field]);
				filePath = XVar.Clone(MVCFunctions.Concat(this.pSet.getUploadFolder((XVar)(this.field)), dbFilename));
				fsFile = XVar.Clone(fs.getFileInfo((XVar)(filePath)));
				if(XVar.Pack(!(XVar)(fsFile)))
				{
					return null;
				}
				fileDesc = XVar.Clone(XVar.Array());
				fileDesc.InitAndSetArrayItem(CommonFunctions.runner_basename((XVar)(dbFilename)), "usrName");
				fileDesc.InitAndSetArrayItem(fsFile["fullPath"], "name");
				fileDesc.InitAndSetArrayItem(CommonFunctions.mimeTypeByExt((XVar)(CommonFunctions.getFileExtension((XVar)(dbFilename)))), "type");
				fileDesc.InitAndSetArrayItem(fsFile["size"], "size");
				if(XVar.Pack(this.pSet.showThumbnail((XVar)(this.field))))
				{
					dynamic fsThumb = XVar.Array(), thumbPath = null;
					thumbPath = XVar.Clone(MVCFunctions.Concat(this.pSet.getUploadFolder((XVar)(this.field)), this.pSet.getStrThumbnail((XVar)(this.field)), dbFilename));
					fsThumb = XVar.Clone(fs.getFileInfo((XVar)(thumbPath)));
					if(XVar.Pack(fsThumb))
					{
						fileDesc.InitAndSetArrayItem(fsThumb["fullPath"], "thumbnail");
						fileDesc.InitAndSetArrayItem(fsThumb["size"], "thumbnail_size");
						fileDesc.InitAndSetArrayItem(fileDesc["type"], "thumbnail_type");
					}
				}
				filesArray.InitAndSetArrayItem(fileDesc, null);
			}
			foreach (KeyValuePair<XVar, dynamic> uploadedFile in filesArray.GetEnumerator())
			{
				if(uploadedFile.Value["usrName"] == filename)
				{
					return uploadedFile.Value;
				}
			}
			return null;
		}
		public virtual XVar showFile(dynamic _param_filename, dynamic _param_thumbnail, dynamic _param_icon, dynamic _param_outputAsAttachment, dynamic _param_useHttpRange, dynamic _param_keys)
		{
			#region pass-by-value parameters
			dynamic filename = XVar.Clone(_param_filename);
			dynamic thumbnail = XVar.Clone(_param_thumbnail);
			dynamic icon = XVar.Clone(_param_icon);
			dynamic outputAsAttachment = XVar.Clone(_param_outputAsAttachment);
			dynamic useHttpRange = XVar.Clone(_param_useHttpRange);
			dynamic keys = XVar.Clone(_param_keys);
			#endregion

			dynamic eTag = null, fileData = XVar.Array(), fs = null, fsFile = XVar.Array();
			if((XVar)(filename == XVar.Pack(""))  && (XVar)(!(XVar)(this.databaseFile())))
			{
				return null;
			}
			fileData = XVar.Clone(this.getFileInfo((XVar)(filename), (XVar)(thumbnail), (XVar)(keys)));
			if((XVar)((XVar)(thumbnail)  && (XVar)(fileData))  && (XVar)(fileData["thumbnail"]))
			{
				fileData.InitAndSetArrayItem(fileData["thumbnail"], "name");
				fileData.InitAndSetArrayItem(fileData["thumbnail_size"], "size");
				fileData.InitAndSetArrayItem(fileData["thumbnail_type"], "type");
			}
			if((XVar)(fileData)  && (XVar)(!(XVar)(this.databaseFile())))
			{
				fs = XVar.Clone(this.getFilesystem());
				fsFile = XVar.Clone(fs.getFileInfo((XVar)(fileData["name"])));
				if((XVar)(!(XVar)(fsFile))  && (XVar)(fs.lastError()))
				{
					MVCFunctions.showError((XVar)(fs.lastError()));
					return null;
				}
				if(XVar.Pack(fsFile))
				{
					fileData.InitAndSetArrayItem(fsFile["size"], "size");
				}
			}
			if((XVar)(!(XVar)(fileData))  || (XVar)((XVar)(!(XVar)(this.databaseFile()))  && (XVar)(!(XVar)(fsFile))))
			{
				if(XVar.Pack(this.imageField()))
				{
					fileData = XVar.Clone(XVar.Array());
					fileData.InitAndSetArrayItem(filename, "usrName");
					fileData.InitAndSetArrayItem(MVCFunctions.myfile_get_contents((XVar)(MVCFunctions.getabspath(new XVar("images/no_image.gif")))), "value");
					fileData.InitAndSetArrayItem("image/gif", "type");
					fileData.InitAndSetArrayItem(MVCFunctions.strlen_bin((XVar)(fileData["value"])), "size");
				}
				else
				{
					return null;
				}
			}
			if(XVar.Pack(icon))
			{
				fileData.InitAndSetArrayItem(MVCFunctions.myfile_get_contents((XVar)(MVCFunctions.getabspath((XVar)(MVCFunctions.Concat("images/icons/", CommonFunctions.getIconByFileType((XVar)(fileData["type"]), (XVar)(fileData["usrName"]))))))), "value");
				fileData.InitAndSetArrayItem(MVCFunctions.strlen_bin((XVar)(fileData["value"])), "size");
				fileData.InitAndSetArrayItem("image/png", "type");
			}
			if(XVar.Pack(!(XVar)(fileData["type"])))
			{
				fileData.InitAndSetArrayItem("application/octet-stream", "type");
			}
			if(XVar.Pack(!(XVar)(fileData["size"])))
			{
				if(XVar.Pack(fileData["value"]))
				{
					fileData.InitAndSetArrayItem(MVCFunctions.strlen_bin((XVar)(fileData["value"])), "size");
				}
				else
				{
					fileData.InitAndSetArrayItem(fsFile["size"], "size");
				}
			}
			if((XVar)((XVar)(!(XVar)(fileData["value"]))  && (XVar)(fsFile))  && (XVar)(!(XVar)(fsFile["returnContent"])))
			{
				dynamic result = null;
				CommonFunctions.add_nocache_headers();
				result = XVar.Clone(fs.redirectToFile((XVar)(fileData["name"]), (XVar)(thumbnail)));
				if(XVar.Equals(XVar.Pack(result), XVar.Pack(false)))
				{
					MVCFunctions.showError((XVar)(fs.lastError()));
				}
				MVCFunctions.ob_flush();
				HttpContext.Current.Response.End();
				throw new RunnerInlineOutputException();
			}
			eTag = XVar.Clone(RunnerFileHandler.calculateETag((XVar)(fileData), (XVar)(fsFile)));
			if(XVar.Pack(eTag))
			{
				MVCFunctions.Header((XVar)(MVCFunctions.Concat("ETag:", eTag)));
				if(XVar.Equals(XVar.Pack(MVCFunctions.GetServerVariable("HTTP_IF_NONE_MATCH")), XVar.Pack(eTag)))
				{
					CommonFunctions.http_response_code(new XVar(304));
					MVCFunctions.ob_flush();
					HttpContext.Current.Response.End();
					throw new RunnerInlineOutputException();
				}
			}
			if((XVar)(useHttpRange)  && (XVar)(MVCFunctions.GetHttpRange()))
			{
				this.showPartialFile((XVar)(fileData), (XVar)(outputAsAttachment));
			}
			else
			{
				MVCFunctions.Header("Accept-Ranges", "none");
				MVCFunctions.Header("Cache-Control", "private, max-age=604800, must-revalidate");
				MVCFunctions.Header((XVar)(MVCFunctions.Concat("Content-Type: ", fileData["type"])));
				MVCFunctions.Header("Access-Control-Allow-Methods", "HEAD, GET, POST");
				if(XVar.Pack(outputAsAttachment))
				{
					MVCFunctions.Header((XVar)(MVCFunctions.Concat("Content-Disposition: attachment;Filename=\"", fileData["usrName"], "\"")));
				}
				MVCFunctions.SendContentLength((XVar)(fileData["size"]));
				if(MVCFunctions.GetServerVariable("REQUEST_METHOD") == "HEAD")
				{
					MVCFunctions.ob_flush();
					HttpContext.Current.Response.End();
					throw new RunnerInlineOutputException();
				}
				if(XVar.Pack(fileData["value"]))
				{
					MVCFunctions.echoBinary((XVar)(fileData["value"]));
				}
				else
				{
					fs.printPartial((XVar)(fileData["name"]));
				}
			}

			return null;
		}
		protected virtual XVar showPartialFile(dynamic _param_fileData, dynamic _param_outputAsAttachment)
		{
			#region pass-by-value parameters
			dynamic fileData = XVar.Clone(_param_fileData);
			dynamic outputAsAttachment = XVar.Clone(_param_outputAsAttachment);
			#endregion

			dynamic httpRange = null, range = null, rangeParts = XVar.Array(), range_orig = null, seek_end = null, seek_start = null, size_unit = null;
			httpRange = XVar.Clone(MVCFunctions.GetHttpRange());
			size_unit = new XVar("");
			range_orig = new XVar("");
			if(XVar.Pack(MVCFunctions.preg_match(new XVar("/^bytes=((\\d*-\\d*,? ?)+)$/"), (XVar)(httpRange))))
			{
				rangeParts = XVar.Clone(MVCFunctions.explode(new XVar("="), (XVar)(httpRange)));
				size_unit = XVar.Clone(rangeParts[0]);
				range_orig = XVar.Clone(rangeParts[1]);
			}
			if(size_unit == "bytes")
			{
				if(!XVar.Equals(XVar.Pack(MVCFunctions.strpos((XVar)(range_orig), new XVar(","))), XVar.Pack(false)))
				{
					dynamic extra_ranges = null;
					rangeParts = XVar.Clone(MVCFunctions.explode(new XVar(","), (XVar)(range_orig)));
					range = XVar.Clone(rangeParts[0]);
					extra_ranges = XVar.Clone(rangeParts[1]);
				}
				else
				{
					range = XVar.Clone(range_orig);
				}
			}
			else
			{
				range = new XVar("-");
			}
			rangeParts = XVar.Clone(MVCFunctions.explode(new XVar("-"), (XVar)(range)));
			seek_start = XVar.Clone(rangeParts[0]);
			seek_end = XVar.Clone(rangeParts[1]);
			seek_end = XVar.Clone((XVar.Pack(MVCFunctions.strlen((XVar)(seek_end)) == 0) ? XVar.Pack(fileData["size"] - 1) : XVar.Pack(MVCFunctions.min((XVar)(MVCFunctions.abs((XVar)(MVCFunctions.intval((XVar)(seek_end))))), (XVar)(fileData["size"] - 1)))));
			seek_start = XVar.Clone((XVar.Pack((XVar)(MVCFunctions.strlen((XVar)(seek_start)) == 0)  || (XVar)(seek_end < MVCFunctions.abs((XVar)(MVCFunctions.intval((XVar)(seek_start)))))) ? XVar.Pack(0) : XVar.Pack(MVCFunctions.max((XVar)(MVCFunctions.abs((XVar)(MVCFunctions.intval((XVar)(seek_start))))), new XVar(0)))));
			MVCFunctions.Header(new XVar("HTTP/1.1 206 Partial Content"));
			MVCFunctions.Header("Accept-Ranges", "bytes");
			MVCFunctions.Header((XVar)(MVCFunctions.Concat("Content-Range: bytes ", seek_start, "-", seek_end, "/", fileData["size"])));
			if(XVar.Pack(outputAsAttachment))
			{
				MVCFunctions.Header((XVar)(MVCFunctions.Concat("Content-Disposition: attachment;Filename=\"", fileData["usrName"], "\"")));
			}
			MVCFunctions.Header((XVar)(MVCFunctions.Concat("Content-Length: ", (seek_end - seek_start) + 1)));
			MVCFunctions.Header("Cache-Control", "cache, must-revalidate");
			MVCFunctions.Header("Pragma", "public");
			MVCFunctions.Header((XVar)(MVCFunctions.Concat("Content-Type: ", fileData["type"])));
			if(MVCFunctions.GetServerVariable("REQUEST_METHOD") == "HEAD")
			{
				MVCFunctions.ob_flush();
				HttpContext.Current.Response.End();
				throw new RunnerInlineOutputException();
			}
			if(XVar.Pack(fileData["value"]))
			{
				MVCFunctions.echoBinaryPartial((XVar)(fileData["value"]), (XVar)(seek_start), (XVar)(seek_end));
			}
			else
			{
				dynamic fs = null;
				fs = XVar.Clone(this.getFilesystem());
				fs.printPartial((XVar)(fileData["name"]), (XVar)(seek_start), (XVar)(seek_end));
			}

			return null;
		}
		protected virtual XVar table()
		{
			return this.pSet.table();
		}
		protected virtual XVar pageType()
		{
			return this.pSet.pageType();
		}
		protected virtual XVar databaseFile()
		{
			dynamic viewFormat = null;
			viewFormat = XVar.Clone(this.pSet.getViewFormat((XVar)(this.field)));
			return (XVar)((XVar)((XVar)(viewFormat == Constants.FORMAT_DATABASE_IMAGE)  || (XVar)(viewFormat == Constants.FORMAT_DATABASE_FILE))  || (XVar)(viewFormat == Constants.FORMAT_DATABASE_AUDIO))  || (XVar)(viewFormat == Constants.FORMAT_DATABASE_VIDEO);
		}
		protected virtual XVar imageField()
		{
			dynamic viewFormat = null;
			viewFormat = XVar.Clone(this.pSet.getViewFormat((XVar)(this.field)));
			return (XVar)(viewFormat == Constants.FORMAT_DATABASE_IMAGE)  || (XVar)(viewFormat == Constants.FORMAT_FILE_IMAGE);
		}
		protected virtual XVar getDataSource()
		{
			if(XVar.Pack(!(XVar)(this._datasource)))
			{
				this._datasource = XVar.Clone(CommonFunctions.getDataSource((XVar)(this.table()), (XVar)(this.pSet)));
			}
			return this._datasource;
		}
		protected virtual XVar getFilesystem()
		{
			if(XVar.Pack(!(XVar)(this._fs)))
			{
				this._fs = XVar.Clone(CommonFunctions.getStorageProvider((XVar)(this.pSet), (XVar)(this.field)));
			}
			return this._fs;
		}
		public static XVar fileType(dynamic _param_uploadedFile)
		{
			#region pass-by-value parameters
			dynamic uploadedFile = XVar.Clone(_param_uploadedFile);
			#endregion

			if(XVar.Pack(uploadedFile["type"]))
			{
				return uploadedFile["type"];
			}
			return CommonFunctions.mimeTypeByExt((XVar)(CommonFunctions.getFileExtension((XVar)(uploadedFile["name"]))));
		}
		protected virtual XVar trimFilename(dynamic _param_name)
		{
			#region pass-by-value parameters
			dynamic name = XVar.Clone(_param_name);
			#endregion

			while(XVar.Pack(this.getFormData((XVar)(name))))
			{
				name = XVar.Clone(MVCFunctions.upcount_name((XVar)(name)));
			}
			return name;
		}
		protected virtual XVar resizeUploadedImage(dynamic _param_uploadedFile, dynamic _param_img_width, dynamic _param_img_height, dynamic _param_size)
		{
			#region pass-by-value parameters
			dynamic uploadedFile = XVar.Clone(_param_uploadedFile);
			dynamic img_width = XVar.Clone(_param_img_width);
			dynamic img_height = XVar.Clone(_param_img_height);
			dynamic size = XVar.Clone(_param_size);
			#endregion

			dynamic newFilename = null, newPath = null, new_height = null, new_width = null, options = null, scale = null, success = null, tempDir = null;
			scale = XVar.Clone(MVCFunctions.min((XVar)(size / img_width), (XVar)(size / img_height)));
			if(1 <= scale)
			{
				return null;
			}
			new_width = XVar.Clone(img_width * scale);
			new_height = XVar.Clone(img_height * scale);
			tempDir = XVar.Clone(this.tempFileDir());
			options = XVar.Clone(XVar.Array());
			newFilename = XVar.Clone(DiskFileSystem.uniqueFilename(new XVar("resize"), (XVar)(tempDir)));
			newPath = XVar.Clone(MVCFunctions.Concat(tempDir, newFilename));
			if(ProjectSettings.ext() == "aspx")
			{
				newPath = XVar.Clone(MVCFunctions.str_replace(new XVar("/"), new XVar("\\"), (XVar)(newPath)));
			}
			success = XVar.Clone(MVCFunctions.imageCreateThumb((XVar)(new_width), (XVar)(new_height), (XVar)(img_width), (XVar)(img_height), (XVar)(uploadedFile["tmp_name"]), (XVar)(options), (XVar)(newPath), (XVar)(uploadedFile)));
			if(XVar.Pack(!(XVar)(success)))
			{
				return null;
			}
			return MVCFunctions.Concat(tempDir, newFilename);
		}
		protected virtual XVar tempFileDir()
		{
			dynamic field = null, providerType = null;
			providerType = XVar.Clone(this.pSet.fileStorageProvider((XVar)(field)));
			if(providerType == Constants.stpDISK)
			{
				return DiskFileSystem.normalizePath((XVar)(this.pSet.getUploadFolder((XVar)(this.field))));
			}
			return MVCFunctions.getabspath(new XVar("templates_c"));
		}
		public virtual XVar getImageDimensions(dynamic _param_uploadedFile)
		{
			#region pass-by-value parameters
			dynamic uploadedFile = XVar.Clone(_param_uploadedFile);
			#endregion

			if((XVar)(this.pSet.getCreateThumbnail((XVar)(this.field)))  || (XVar)(this.pSet.getResizeOnUpload((XVar)(this.field))))
			{
				return MVCFunctions.runner_getimagesize((XVar)(uploadedFile["tmp_name"]), (XVar)(uploadedFile));
			}
			return XVar.Array();
		}
		public virtual XVar codeToMessage(dynamic _param_code)
		{
			#region pass-by-value parameters
			dynamic code = XVar.Clone(_param_code);
			#endregion

			dynamic message = null;
			switch(((XVar)code).ToInt())
			{
				case 1:
					message = new XVar("UPLOAD_ERR_INI_SIZE: The uploaded file exceeds the upload_max_filesize directive in php.ini");
					break;
				case 2:
					message = new XVar("UPLOAD_ERR_FORM_SIZE: The uploaded file exceeds the MAX_FILE_SIZE directive that was specified in the HTML form");
					break;
				case 3:
					message = new XVar("UPLOAD_ERR_PARTIAL: The uploaded file was only partially uploaded");
					break;
				case 4:
					message = new XVar("UPLOAD_ERR_NO_FILE: No file was uploaded");
					break;
				case 6:
					message = new XVar("UPLOAD_ERR_NO_TMP_DIR: Missing a temporary folder");
					break;
				case 7:
					message = new XVar("UPLOAD_ERR_CANT_WRITE: Failed to write file to disk");
					break;
				case 8:
					message = new XVar("UPLOAD_ERR_EXTENSION: File upload stopped by extension");
					break;
				default:
					message = new XVar("Unknown upload error");
					break;
			}
			return message;
		}
		public virtual XVar directUpload()
		{
			dynamic fs = null;
			fs = XVar.Clone(this.getFilesystem());
			return fs.directUpload();
		}
		public virtual XVar prepareDirectUpload(dynamic _param_fileData)
		{
			#region pass-by-value parameters
			dynamic fileData = XVar.Clone(_param_fileData);
			#endregion

			dynamic errorMessage = null, file = XVar.Array(), result = XVar.Array();
			result = XVar.Clone(XVar.Array());
			file = XVar.Clone(XVar.Array());
			if(XVar.Pack(!(XVar)(this.validateFile((XVar)(fileData["name"]), (XVar)(fileData["size"]), ref errorMessage))))
			{
				file.InitAndSetArrayItem(errorMessage, "error");
			}
			else
			{
				dynamic fs = null, uploadInfo = XVar.Array();
				fs = XVar.Clone(this.getFilesystem());
				uploadInfo = XVar.Clone(fs.initUpload((XVar)(fileData["name"])));
				if(XVar.Pack(uploadInfo))
				{
					file.InitAndSetArrayItem(this.trimFilename((XVar)(fileData["name"])), "usrName");
					file.InitAndSetArrayItem(fileData["size"], "size");
					file.InitAndSetArrayItem(fileData["type"], "type");
					file.InitAndSetArrayItem(uploadInfo["fileId"], "name");
					result.InitAndSetArrayItem(uploadInfo["uploadParams"], "uploadParams");
				}
				else
				{
					file.InitAndSetArrayItem(fs.lastError(), "error");
				}
			}
			if(XVar.Pack(!(XVar)(file.KeyExists("error"))))
			{
				fileData = XVar.Clone(XVar.Array());
				fileData.InitAndSetArrayItem(file, "file");
				fileData.InitAndSetArrayItem(false, "deleted");
				fileData.InitAndSetArrayItem(false, "fromDB");
				this.setFormData((XVar)(file["usrName"]), (XVar)(fileData));
				result.InitAndSetArrayItem(this.returnFileDescriptor((XVar)(file)), "file");
			}
			else
			{
				result.InitAndSetArrayItem(file["error"], "error");
			}
			MVCFunctions.Header("Vary", "Accept");
			MVCFunctions.Header("Content-type", "application/json");
			MVCFunctions.Echo(MVCFunctions.runner_json_encode((XVar)(result)));
			MVCFunctions.ob_flush();
			HttpContext.Current.Response.End();
			throw new RunnerInlineOutputException();

			return null;
		}
		public static XVar calculateETag(dynamic fileData, dynamic fsFile)
		{
			if((XVar)(!(XVar)(fileData["value"]))  && (XVar)(fsFile))
			{
				if(XVar.Pack(fsFile["lastModified"]))
				{
					return MVCFunctions.md5((XVar)(MVCFunctions.Concat(fileData["name"], fsFile["lastModified"])));
				}
			}
			else
			{
				if(XVar.Pack(fileData["value"]))
				{
					dynamic chunkSize = null, fileSize = null;
					fileSize = XVar.Clone(MVCFunctions.strlen_bin((XVar)(fileData["value"])));
					chunkSize = XVar.Clone((XVar.Pack(2000 < fileSize) ? XVar.Pack(2000) : XVar.Pack(fileSize)));
					return MVCFunctions.md5((XVar)(MVCFunctions.Concat(fileSize, MVCFunctions.substr((XVar)(fileData["value"]), new XVar(0), (XVar)(chunkSize)))));
				}
			}
			return "";
		}
		public virtual XVar loadFiles(dynamic _param_filesArray)
		{
			#region pass-by-value parameters
			dynamic filesArray = XVar.Clone(_param_filesArray);
			#endregion

			dynamic userFilesArray = XVar.Array();
			userFilesArray = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> file in filesArray.GetEnumerator())
			{
				dynamic sessionArray = XVar.Array(), userFile = null;
				sessionArray = XVar.Clone(XVar.Array());
				sessionArray.InitAndSetArrayItem(file.Value, "file");
				sessionArray.InitAndSetArrayItem(true, "fromDB");
				sessionArray.InitAndSetArrayItem(false, "deleted");
				this.setFormData((XVar)(file.Value["usrName"]), (XVar)(sessionArray));
				userFile = XVar.Clone(this.returnFileDescriptor((XVar)(file.Value)));
				userFilesArray.InitAndSetArrayItem(userFile, null);
			}
			return userFilesArray;
		}
		public static XVar getFileArray(dynamic _param_value, dynamic _param_field, dynamic _param_pSet_packed)
		{
			#region packeted values
			ProjectSettings _param_pSet = XVar.UnPackProjectSettings(_param_pSet_packed);
			#endregion

			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			dynamic field = XVar.Clone(_param_field);
			ProjectSettings pSet = XVar.Clone(_param_pSet);
			#endregion

			dynamic fileData = XVar.Array(), files = null, path = null;
			if((XVar)(!(XVar)(value))  || (XVar)(value == "[]"))
			{
				return XVar.Array();
			}
			files = XVar.Clone(MVCFunctions.runner_json_decode((XVar)(value)));
			if(XVar.Pack(files))
			{
				return files;
			}
			path = XVar.Clone(DiskFileSystem.normalizeRelativePath((XVar)(pSet.getUploadFolder((XVar)(field)))));
			fileData = XVar.Clone(XVar.Array());
			fileData.InitAndSetArrayItem(CommonFunctions.runner_basename((XVar)(value)), "usrName");
			fileData.InitAndSetArrayItem(MVCFunctions.Concat(path, value), "name");
			if(XVar.Pack(pSet.showThumbnail((XVar)(field))))
			{
				dynamic thumbprefix = null;
				thumbprefix = XVar.Clone(pSet.getStrThumbnail((XVar)(field)));
				fileData.InitAndSetArrayItem(MVCFunctions.Concat(path, thumbprefix, value), "thumbnail");
			}
			return new XVar(0, fileData);
		}
		protected virtual XVar securityCondition()
		{
			dynamic conditions = XVar.Array(), rights = XVar.Array();
			rights = XVar.Clone(new XVar(0, "S", 1, "P", 2, "E"));
			conditions = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> r in rights.GetEnumerator())
			{
				dynamic cond = null;
				cond = XVar.Clone(Security.SelectCondition((XVar)(r.Value), (XVar)(this.pSet)));
				if(XVar.Pack(!(XVar)(cond)))
				{
					return null;
				}
				conditions.InitAndSetArrayItem(cond, null);
			}
			return DataCondition._Or((XVar)(conditions));
		}
		protected virtual XVar tableRightsAvailable()
		{
			dynamic mask = null, rights = XVar.Array();
			rights = XVar.Clone(new XVar(0, "S", 1, "P", 2, "E"));
			mask = XVar.Clone(CommonFunctions.GetUserPermissions((XVar)(this.table())));
			foreach (KeyValuePair<XVar, dynamic> r in rights.GetEnumerator())
			{
				if(!XVar.Equals(XVar.Pack(MVCFunctions.strpos((XVar)(mask), (XVar)(r.Value))), XVar.Pack(false)))
				{
					return true;
				}
			}
			return false;
		}
	}
}
